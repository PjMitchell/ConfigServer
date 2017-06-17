using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting
{
    public class UploadToEditorModelMapperTests
    {
        private Mock<IConfigurationEditModelMapper> configurationEditModelMapper;
        private Mock<IConfigurationUploadMapper> uploadMapper;
        private IUploadToEditorModelMapper target;
        private ConfigurationIdentity inputedId;
        private ConfigurationModel model;


        public UploadToEditorModelMapperTests()
        {
            configurationEditModelMapper = new Mock<IConfigurationEditModelMapper>();
            uploadMapper = new Mock<IConfigurationUploadMapper>();
            target = new UploadToEditorModelMapper(configurationEditModelMapper.Object, uploadMapper.Object);
            inputedId = new ConfigurationIdentity(new ConfigurationClient { ClientId = Guid.NewGuid().ToString() }, new Version(1, 0));
            model = new ConfigurationModel<SimpleConfig, SimpleConfigSet>("Test", c => c.Config, (cs, c) => cs.Config = c);
        }

        [Fact]
        public void Map_MapsUpload()
        {
            var configInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, inputedId);
            var input = "Input";
            var expectedResult = new Object();
            uploadMapper.Setup(m => m.MapToConfigInstance(input, inputedId, model))
                .Returns(configInstance);
            configurationEditModelMapper.Setup(m => m.MapToEditConfig(configInstance, model))
                .Returns(expectedResult);
            var result = target.MapUploadToEditModel(input, inputedId, model);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Map_MapsUploadIfUploadNull()
        {
            var configInstance = new ConfigInstance<SimpleConfig>(null, inputedId);
            var input = "Input";
            var expectedResult = new Object();
            uploadMapper.Setup(m => m.MapToConfigInstance(input, inputedId, model))
                .Returns(configInstance);
            configurationEditModelMapper.Setup(m => m.MapToEditConfig(It.Is<ConfigInstance<SimpleConfig>>(s => s.Configuration != null), model))
                .Returns(expectedResult);
            var result = target.MapUploadToEditModel(input, inputedId, model);
            Assert.Equal(expectedResult, result);
        }
    }
}
