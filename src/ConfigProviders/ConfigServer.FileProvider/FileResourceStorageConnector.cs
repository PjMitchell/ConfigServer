using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// File resource storage connector
    /// </summary>
    public class FileResourceStorageConnector : IFileResourceStorageConnector
    {
        readonly string folderPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public FileResourceStorageConnector(FileResourceRepositoryBuilderOptions option)
        {
            this.folderPath = option.ResourceStorePath;
        }

        /// <summary>
        /// Get Resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Task<byte[]> GetResourceAsync(string resourceName, string instanceId)
        {
            string path = Path.Combine(GetResourceSetFolder(instanceId).FullName, resourceName);
            byte[] resource;
            try
            {
                resource = File.ReadAllBytes(path);
            }
            catch
            {
                resource = new byte[0];
            }

            return Task.FromResult(resource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetResourceCatalog(string instanceId)
        {
            string path = GetResourceSetFolder(instanceId).FullName;
            return Task.FromResult(Directory.EnumerateFiles(path).Select(f => Path.GetFileName(f)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="resource"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Task SetResourceAsync(string resourceName, byte[] resource, string instanceId)
        {
            string path = Path.Combine(GetResourceSetFolder(instanceId).FullName, resourceName);
            File.WriteAllBytes(path, resource);

            return Task.FromResult(true);
        }

        private DirectoryInfo GetResourceStore()
        {
            return Directory.CreateDirectory(folderPath);
        }

        private DirectoryInfo GetResourceSetFolder(string clientId)
        {
            var filestore = GetResourceStore();
            return filestore.EnumerateDirectories().SingleOrDefault(s => s.Name == clientId) ?? filestore.CreateSubdirectory(clientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceInstanceId"></param>
        /// <param name="destinationInstanceId"></param>
        /// <returns></returns>
        public Task CopyResourcesAsync(string sourceInstanceId, string destinationInstanceId)
        {
            var originPath = GetResourceSetFolder(sourceInstanceId);
            var destinationPath = GetResourceSetFolder(destinationInstanceId);
            foreach (var originFile in Directory.EnumerateFiles(originPath.FullName))
            {
                string destinationFile = Path.Combine(destinationPath.FullName, Path.GetFileName(originFile));

                File.Copy(originFile, destinationFile);
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Task DeleteResources(string resourceName, string instanceId)
        {
            var path = GetResourceSetFolder(instanceId);
            var fileToDelete = Path.Combine(path.FullName, resourceName);
            File.Delete(fileToDelete);
            return Task.FromResult(true);
        }
    }
}
