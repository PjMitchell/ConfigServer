using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class FileResourceConnector : IFileResourceStorageConnector
    {
        readonly string folderPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public FileResourceConnector(FileResourceRepositoryBuilderOptions option)
        {
            this.folderPath = option.ResourceStorePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Task<byte[]> GetResourceAsync(string resourceName, string instanceId)
        {
            string path = Path.Combine(GetResourceSetFolder(instanceId).FullName, resourceName);
            byte[] resource = File.ReadAllBytes(path);

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
            return Task.FromResult(Directory.EnumerateFiles(path).Select(f=> Path.GetFileName(f)));
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

            return Task.FromResult(true);        }

        private DirectoryInfo GetResourceStore()
        {
            return Directory.CreateDirectory(folderPath);
        }

        private DirectoryInfo GetResourceSetFolder(string clientId)
        {
            var filestore = GetResourceStore();
            return filestore.EnumerateDirectories().SingleOrDefault(s => s.Name == clientId) ?? filestore.CreateSubdirectory(clientId);
        }
    }
}
