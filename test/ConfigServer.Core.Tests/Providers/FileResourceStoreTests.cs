using ConfigServer.FileProvider;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class FileResourceStoreTests : IDisposable
    {
        private readonly IResourceStore target;
        private readonly string testdirectory;
        private readonly ConfigurationClient client;
        private readonly ConfigurationIdentity configId;


        public FileResourceStoreTests()
        {
            testdirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/TestOutput/{Guid.NewGuid()}";
            var option = new FileResourceRepositoryBuilderOptions { ResourceStorePath = testdirectory };

            target = new FileResourceStore(option);
            client = new ConfigurationClient
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4",
                Name = "Client 1",
                Description = "A description Client"
            };
            configId = new ConfigurationIdentity(client, new Version(1, 0));
        }

        public void Dispose()
        {
            var di = new DirectoryInfo(testdirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Directory.Delete(testdirectory);
        }

        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            byte[] resource = new byte[128];


            UpdateResourceRequest request = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 128.png",
                Content = new MemoryStream(resource)
            };

            await target.UpdateResource(request);


            using (var result = await target.GetResource(request.Name, configId))
            {
                Assert.Equal(resource.LongCount(), result.Content.Length);
            }
        }

        [Fact]
        public async Task GetListOfResources()
        {
            byte[] resource = new byte[128];

            UpdateResourceRequest request1 = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 1.png",
                Content = new MemoryStream(resource)
            };

            UpdateResourceRequest request2 = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 2.png",
                Content = new MemoryStream(resource)
            };

            await target.UpdateResource(request1);
            await target.UpdateResource(request2);

            var result = await target.GetResourceCatalogue(configId);

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.Where(f => f.Name == request1.Name).Count());
            Assert.Equal(1, result.Where(f => f.Name == request2.Name).Count());

        }
        [Fact]
        public async void CanUpdate()
        {
            byte[] resource = new byte[128];

            UpdateResourceRequest request = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 128.png",
                Content = new MemoryStream(resource)
            };

            await target.UpdateResource(request);

            byte[] resourceUpdated = new byte[16];

            UpdateResourceRequest requestUpdated = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 128.png",
                Content = new MemoryStream(resourceUpdated)
            };

            await target.UpdateResource(requestUpdated);
            
            using (var result = await target.GetResource(request.Name, configId))
            {
                Assert.Equal(resourceUpdated.LongCount(), result.Content.Length);
            }

        }

        [Fact]
        public async void CopyResources()
        {
            byte[] resource = new byte[128];

            UpdateResourceRequest request = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 128.png",
                Content = new MemoryStream(resource)
            };
            await target.UpdateResource(request);

            var client2 = new ConfigurationClient
            {
                ClientId = "3FEE5A18-A00F-4565-BEFE-C79E0823F6D4",
                Name = "Client 2",
                Description = "A description Client"
            };

            var configId2 = new ConfigurationIdentity(client2, new Version(1, 0));


            await target.CopyResources(configId, configId2);


            var result = await target.GetResourceCatalogue(configId2);

            Assert.Equal(1, result.Count());
            Assert.Equal(request.Name, result.First().Name);
        }

        [Fact]
        public async void DeleteResources()
        {
            byte[] resource = new byte[128];

            UpdateResourceRequest request = new UpdateResourceRequest()
            {
                Identity = configId,
                Name = "Resource 128.png",
                Content = new MemoryStream(resource)
            };
            await target.UpdateResource(request);

            await target.DeleteResources(request.Name, configId);


            var result = await target.GetResourceCatalogue(configId);

            Assert.Equal(0, result.Where(f => f.Name == request.Name).Count());

        }
    }
}
