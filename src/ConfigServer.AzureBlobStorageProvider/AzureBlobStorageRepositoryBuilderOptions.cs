using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using System;

namespace ConfigServer.AzureBlobStorageProvider
{
    /// <summary>
    /// Azure Blob Storage Repository Builder Options
    /// </summary>
    public class AzureBlobStorageRepositoryBuilderOptions
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
        /// Default ConfigServerStore
        /// </summary>
        public string Container { get; set; } = "ConfigServerStore";

        /// <summary>
        /// Json Serialization settings
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
    }
}
