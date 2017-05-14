using ConfigServer.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;

namespace ConfigServer.FileProvider
{

    internal class FileResourceStore : IResourceStore
    {
        readonly string folderPath;

        public FileResourceStore(FileResourceRepositoryBuilderOptions options)
        {
            folderPath = options.ResourceStorePath;
        }

        public Task<ResourceEntry> GetResource(string name, ConfigurationIdentity identity)
        {
          
            string path = Path.Combine(GetResourceSetFolder(identity).FullName, name);

            var result = new ResourceEntry()
            {
                Name = name,
                Content = File.OpenRead(path),
                HasEntry = File.Exists(path)
            };
            return Task.FromResult(result);

        }

        public Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            string path = GetResourceSetFolder(identity).FullName;
            var result = Directory.EnumerateFiles(path)
                .Select(MapEntryInfo);

            return Task.FromResult(result);
        }

        public Task UpdateResource(UpdateResourceRequest request)
        {
            string path = Path.Combine(GetResourceSetFolder(request.Identity).FullName, request.Name);
            var existingFile = new FileInfo(path);
            ArchiveIfExists(existingFile, request.Identity);
            using (var writeStream = File.Create(path))
            {
                request.Content.Seek(0, SeekOrigin.Begin);
                request.Content.CopyTo(writeStream);
            }

            return Task.FromResult(true);
        }

        public Task CopyResources(ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            var originPath = GetResourceSetFolder(sourceIdentity);
            var destinationPath = GetResourceSetFolder(destinationIdentity);
            foreach (var originFile in Directory.EnumerateFiles(originPath.FullName))
            {
                CopyResource(destinationPath, originFile, destinationIdentity);
            }
            return Task.FromResult(true);
        }

        public Task CopyResources(IEnumerable<string> filesToCopy, ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            var fileHashSet = new HashSet<string>(filesToCopy, StringComparer.OrdinalIgnoreCase);
            var originPath = GetResourceSetFolder(sourceIdentity);
            var destinationPath = GetResourceSetFolder(destinationIdentity);
            foreach (var originFile in Directory.EnumerateFiles(originPath.FullName).Where(w => filesToCopy.Contains(Path.GetFileName(w))))
            {
                CopyResource(destinationPath, originFile, destinationIdentity);
            }
            return Task.FromResult(true);
        }

        public Task DeleteResources(string name, ConfigurationIdentity identity)
        {
            var path = GetResourceSetFolder(identity);
            var fileToDelete = Path.Combine(path.FullName, name);
            var existingFile = new FileInfo(fileToDelete);
            ArchiveIfExists(existingFile, identity);
            existingFile.Delete();
            return Task.FromResult(true);
        }

        private void CopyResource(DirectoryInfo destinationPath, string originFilePath, ConfigurationIdentity identity)
        {
            var existingFile = new FileInfo(originFilePath);

            string destinationFile = Path.Combine(destinationPath.FullName, existingFile.Name);
            ArchiveIfExists(new FileInfo(destinationFile), identity);
            existingFile.CopyTo(destinationFile);
        }

        private void ArchiveIfExists(FileInfo existingFile, ConfigurationIdentity identity)
        {
            if (!existingFile.Exists)
                return;
            var newName = $"{Path.GetFileNameWithoutExtension(existingFile.FullName)}_{existingFile.LastWriteTimeUtc.ToString("yyMMddHHmmssff")}{existingFile.Extension}";
            var newPath = Path.Combine(GetArchiveResourceSetFolder(identity).FullName, newName);
            File.Copy(existingFile.FullName, newPath);
        }

        private ResourceEntryInfo MapEntryInfo(string path)
        {
            var fileinfo = new FileInfo(path);
            return new ResourceEntryInfo
            {
                Name = fileinfo.Name,
                TimeStamp = fileinfo.LastWriteTimeUtc
            };
        }

        private DirectoryInfo GetResourceSetFolder(ConfigurationIdentity identity)
        {
            var filestore = Directory.CreateDirectory($"{folderPath}/{identity.Client.ClientId}");
            return filestore;
        }

        private DirectoryInfo GetArchiveResourceSetFolder(ConfigurationIdentity identity)
        {
            var filestore = Directory.CreateDirectory($"{folderPath}/Archive/{identity.Client.ClientId}");
            return filestore;
        }
    }
}
