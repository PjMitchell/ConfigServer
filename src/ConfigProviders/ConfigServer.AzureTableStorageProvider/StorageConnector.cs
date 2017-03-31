using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using ConfigServer.TextProvider.Core;
using System;

namespace ConfigServer.AzureTableStorageProvider
{
    internal class StorageConnector : IStorageConnector
    {
        private readonly CloudTableClient client;
        private readonly string table;
        private const string clientRegistry = "clientRegistry";
        private const string clientGroupRegistry = "clientGroupRegistry";


        public StorageConnector(AzureTableStorageRepositoryBuilderOptions options)
        {
            client = new CloudTableClient(options.Uri, options.Credentials);
            this.table = options.Table;
        }

        public Task<string> GetConfigFileAsync(string configId, string instanceId)
        {
            return GetFileAsync(instanceId, configId);
        }
        
        public Task SetConfigFileAsync(string configId, string instanceId, string value)
        {
            return SetFileAsync(instanceId, configId, value);
        }

        public Task<string> GetClientRegistryFileAsync()
        {
            return GetFileAsync(clientRegistry, clientRegistry);
        }

        public Task SetClientRegistryFileAsync(string value)
        {
            return SetFileAsync(clientRegistry, clientRegistry, value);
        }

        public Task<string> GetClientGroupRegistryFileAsync()
        {
            return GetFileAsync(clientGroupRegistry, clientGroupRegistry);
        }

        public Task SetClientGroupRegistryFileAsync(string value)
        {
            return SetFileAsync(clientGroupRegistry, clientGroupRegistry, value);
        }

        private async Task<string> GetFileAsync(string partition, string rowId)
        {
            var tableRef = client.GetTableReference(table);
            var retrieveOperation = TableOperation.Retrieve<TextTableEntry>(partition, rowId);

            var result = await tableRef.ExecuteAsync(retrieveOperation);
            var entry = result?.Result as TextTableEntry;
            return entry?.Value;
        }

        private async Task SetFileAsync(string partition, string rowId, string value)
        {
            var tableRef = client.GetTableReference(table);
            var entry = new TextTableEntry
            {
                PartitionKey = partition,
                RowKey = rowId,
                Value = value
            };
            var retrieveOperation = TableOperation.InsertOrReplace(entry);
            await tableRef.ExecuteAsync(retrieveOperation);
        }


    }
}
