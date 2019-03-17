using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.IntegrationTests
{
    public class EndPointTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public EndPointTest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<FileStartUp>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task CanGetClients()
        {
            var response = await _client.GetAsync("/Manager/Api/Clients");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Assert.True(json.Length > 2, "Expecting content for clients");
        }

        [Fact]
        public async Task CanGetConfig()
        {
            var response = await _client.GetAsync("/3E37AC18-A00F-47A5-B84E-C79E0823F6D4/SampleConfig");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Assert.True(json.Length > 2, "Expecting content for clients");
        }
    }
}
