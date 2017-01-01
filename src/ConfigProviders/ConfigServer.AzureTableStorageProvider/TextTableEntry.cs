using Microsoft.WindowsAzure.Storage.Table;

namespace ConfigServer.AzureTableStorageProvider
{
    internal class TextTableEntry :TableEntity
    {
        public string Value { get; set; }
    }
}
