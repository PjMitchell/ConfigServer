using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    internal class FileResourceArchive : IResourceArchive
    {
        readonly string folderPath;

        public FileResourceArchive(FileResourceRepositoryBuilderOptions options)
        {
            folderPath = options.ResourceStorePath;
        }

        public Task DeleteArchiveResource(string name, ConfigurationIdentity identity)
        {
            var path = GetArchiveResourceSetFolder(identity);
            var fileToDelete = Path.Combine(path.FullName, name);
            var existingFile = new FileInfo(fileToDelete);
            existingFile.Delete();
            return Task.FromResult(true);
        }

        public Task DeleteOldArchiveResources(DateTime deletionDate, ConfigurationIdentity identity)
        {
            string path = GetArchiveResourceSetFolder(identity).FullName;
            var filesToDelete = Directory.EnumerateFiles(path)
                .Select(s => new FileInfo(s))
                .Where(f => f.LastWriteTimeUtc <= deletionDate);

            foreach (var fileToDelete in filesToDelete)
                fileToDelete.Delete();

            return Task.FromResult(true);
        }

        public Task<ResourceEntry> GetArchiveResource(string name, ConfigurationIdentity identity)
        {
            string path = Path.Combine(GetArchiveResourceSetFolder(identity).FullName, name);

            var result = new ResourceEntry()
            {
                Name = name,
                Content = File.OpenRead(path),
                HasEntry = File.Exists(path)
            };
            return Task.FromResult(result); 
        }

        public Task<IEnumerable<ResourceEntryInfo>> GetArchiveResourceCatalogue(ConfigurationIdentity identity)
        {
            string path = GetArchiveResourceSetFolder(identity).FullName;
            var result = Directory.EnumerateFiles(path)
                .Select(MapEntryInfo);

            return Task.FromResult(result);
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

        private DirectoryInfo GetArchiveResourceSetFolder(ConfigurationIdentity identity)
        {
            var filestore = Directory.CreateDirectory($"{folderPath}/Archive/{identity.Client.ClientId}");
            return filestore;
        }
    }
}
