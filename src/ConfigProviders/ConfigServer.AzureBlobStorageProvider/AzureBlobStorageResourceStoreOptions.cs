using Microsoft.WindowsAzure.Storage.Auth;
using System;

namespace ConfigServer.AzureBlobStorageProvider
{
    /// <summary>
    /// Azure Blob Storage Resource Store Options
    /// </summary>
    public class AzureBlobStorageResourceStoreOptions
    {
        /// <summary>
        /// Azure storage base url
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Azure storage blob storage credentials
        /// </summary>
        public StorageCredentials Credentials { get; set; }

        /// <summary>
        /// Azure storage blob container used
        /// Default ConfigServerResourceStore
        /// </summary>
        public string Container { get; set; } = "ConfigServerResourceStore";
    }
}
