using ConfigServer.Server;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.ConfigurationServices
{
    public class ConfigurationClientServiceTest
    {
        readonly private Mock<IConfigClientRepository> clientRepo;
        readonly private IMemoryCache memoryCache;
        readonly private IConfigurationClientService target;
        readonly private IEnumerable<ConfigurationClient> clients;
        readonly private IEnumerable<ConfigurationClientGroup> groups;
        const string clientOne =   "46e9e680-f27f-4a14-bdb8-ecfaaf7541f2";
        const string clientTwo =   "48d4a512-8170-402b-9bf2-fd6cbd8e003c";
        const string clientThree = "b25ac84b-fdca-458f-8935-b6339466e3f4";
        const string groupOne =    "8168de6e-03b9-4344-8d86-21a188aa73eb";
        const string groupTwo = "b9b256da-3f7b-4a95-bc9d-2ee5e6f48b24";

        public ConfigurationClientServiceTest()
        {
            clients = BuildClients();
            groups = BuildGroups();
            clientRepo = new Mock<IConfigClientRepository>();
            clientRepo.Setup(r => r.GetClientsAsync()).ReturnsAsync(() => clients);
            clientRepo.Setup(r => r.GetClientGroupsAsync()).ReturnsAsync(() => groups);

            memoryCache = new MemoryCache(Microsoft.Extensions.Options.Options.Create<MemoryCacheOptions>(new MemoryCacheOptions()));
            target = new ConfigurationClientService(clientRepo.Object, memoryCache);
        }

        [Fact]
        public async Task CanGetClients()
        {
            var result = await target.GetClients();
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task CanGetClient()
        {
            var result = await target.GetClientOrDefault(clientOne);
            Assert.Equal(clientOne, result.ClientId);
            Assert.Equal("One", result.Name);
            Assert.Equal(groupOne, result.Group);
        }

        [Fact]
        public async Task ReturnsNullIfClientNotFound()
        {
            var result = await target.GetClientOrDefault(groupOne);
            Assert.Null(result);
        }

        [Fact]
        public async Task CanGetClientGroups()
        {
            var result = await target.GetGroups();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CanGetClientGroup()
        {
            var result = await target.GetClientGroupOrDefault(groupOne);
            Assert.Equal(groupOne, result.GroupId);
            Assert.Equal("Group One", result.Name);
        }

        [Fact]
        public async Task ReturnsNullIfClientGroupNotFound()
        {
            var result = await target.GetClientGroupOrDefault(clientOne);
            Assert.Null(result);
        }

        private IEnumerable<ConfigurationClient> BuildClients()
        {
            return new List<ConfigurationClient>
            {
                new ConfigurationClient { ClientId = clientOne, Name = "One", Group = groupOne  },
                new ConfigurationClient { ClientId = clientTwo, Name = "Two", Group = groupTwo  },
                new ConfigurationClient { ClientId = clientThree, Name = "Three", Group = ""  }
            };
        }

        private IEnumerable<ConfigurationClientGroup> BuildGroups()
        {
            return new List<ConfigurationClientGroup>
            {
                new ConfigurationClientGroup { GroupId = groupOne, Name = "Group One"  },
                new ConfigurationClientGroup { GroupId = groupTwo, Name = "Group Two"  }
            };
        }
    }
}
