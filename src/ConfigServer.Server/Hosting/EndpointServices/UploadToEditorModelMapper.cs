using ConfigServer.Core;

namespace ConfigServer.Server
{
    internal interface IUploadToEditorModelMapper
    {
        object MapUploadToEditModel(string json, ConfigurationIdentity identity, ConfigurationModel model);
    }

    internal class UploadToEditorModelMapper : IUploadToEditorModelMapper
    {
        private IConfigurationEditModelMapper configurationEditModelMapper;
        private IConfigurationUploadMapper uploadMapper;

        public UploadToEditorModelMapper(IConfigurationEditModelMapper configurationEditModelMapper, IConfigurationUploadMapper uploadMapper)
        {
            this.configurationEditModelMapper = configurationEditModelMapper;
            this.uploadMapper = uploadMapper;
        }

        public object MapUploadToEditModel(string json, ConfigurationIdentity identity, ConfigurationModel model)
        {
            var uploadedInstance = uploadMapper.MapToConfigInstance(json, identity, model);
            if (uploadedInstance.GetConfiguration() == null)
                uploadedInstance.SetConfiguration(uploadedInstance.ConstructNewConfiguration());
            return configurationEditModelMapper.MapToEditConfig(uploadedInstance, model);
        }
    }
}
