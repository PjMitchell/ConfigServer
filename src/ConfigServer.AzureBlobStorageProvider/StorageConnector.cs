using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace ConfigServer.AzureBlobStorageProvider
{
    /// <summary>
    /// Interface for storage connector
    /// </summary>
    public interface IStorageConnector
    {
        /// <summary>
        /// Gets File from storage
        /// </summary>
        /// <param name="location">location of file in storage</param>
        /// <returns>Text from file</returns>
        Task<string> GetFileAsync(string location);

        /// <summary>
        /// Set File in storage
        /// </summary>
        /// <param name="location">location of file in storage</param>
        /// <param name="value">new value</param>
        /// <returns>Task from operation</returns>
        Task SetFileAsync(string location, string value);
    }

    internal class StorageConnector : IStorageConnector
    {
        private readonly CloudBlobClient client;
        private readonly string container;

        public StorageConnector(AzureBlobStorageRepositoryBuilderOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            this.container = options.Container;
        }

        public async Task<string> GetFileAsync(string location)
        {
            var containerRef = client.GetContainerReference(container);
            ICloudBlob entry = await containerRef.GetBlobReferenceFromServerAsync(location);
            using (var stream = new MemoryStream())
            {
                await entry.DownloadToStreamAsync(stream);
                var streamReader = new StreamReader(stream);
                return await streamReader.ReadToEndAsync();
            }              
        }

        public async Task SetFileAsync(string location, string value)
        {
            var containerRef = client.GetContainerReference(container);
            ICloudBlob entry = await containerRef.GetBlobReferenceFromServerAsync(location);
            using (var stream = new MemoryStream())
            {
                var streamReader = new StreamWriter(stream);
                await streamReader.WriteAsync(value);
                await entry.UploadFromStreamAsync(stream);
            }
        }
    }
}
