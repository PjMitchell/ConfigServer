using ConfigServer.AzureBlobStorageProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core.Tests.Providers
{
    public class TestStorageConnectors : IStorageConnector
    {
        private readonly Dictionary<string, string> store;

        public TestStorageConnectors()
        {
            store = new Dictionary<string, string>();
        }

        public Task<string> GetFileAsync(string location)
        {
            string result;
            if(!store.TryGetValue(location, out result))
                result = string.Empty;

            return Task.FromResult<string>(result);
        }

        public Task SetFileAsync(string location, string value)
        {
            store[location] = value;

            return Task.FromResult<bool>(true);
        }
    }
}
