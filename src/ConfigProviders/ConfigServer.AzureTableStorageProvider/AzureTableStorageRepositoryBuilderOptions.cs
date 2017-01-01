using ConfigServer.TextProvider.Core;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using System;

namespace ConfigServer.AzureTableStorageProvider
{
    /// <summary>
    /// Azure Table Storage Repository Builder Options
    /// </summary>
    public class AzureTableStorageRepositoryBuilderOptions : ITextStorageSetting
    {
        /// <summary>
        /// Azure storage base url
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Azure storage table storage credentials
        /// </summary>
        public StorageCredentials Credentials { get; set; }
        
        /// <summary>
        /// Azure storage table used
        /// Default ConfigServerStore
        /// </summary>
        public string Table { get; set; } = "ConfigServerStore";

        /// <summary>
        /// Json Serialization settings
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
    }
}
