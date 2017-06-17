using ConfigServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal interface IConfigurationUploadMapper
    {
        object ToObjectOrDefault(string value, ConfigurationModel model);
        ConfigInstance MapToConfigInstance(string value, ConfigurationIdentity identity, ConfigurationModel model);
    }

    internal class ConfigurationUploadMapper : IConfigurationUploadMapper
    {
        public object ToObjectOrDefault(string token, ConfigurationModel model)
        {
            if (model is ConfigurationOptionModel option)
                return ToObjectOrDefault(token, option.StoredType);
            return ToObjectOrDefault(token, model.Type);
        }

        public ConfigInstance MapToConfigInstance(string value, ConfigurationIdentity identity, ConfigurationModel model)
        {
            var config = ToObjectOrDefault(value, model);
            return BuildConfigInstance(identity, model, config);
        }

        private object ToObjectOrDefault(string token, Type type)
        {
            bool failed = false;
            var result = JsonConvert.DeserializeObject(token.ToString(), type, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    failed = true;
                    args.ErrorContext.Handled = true;
                }
            });
            return failed ? null : result;
        }

        private static ConfigInstance BuildConfigInstance(ConfigurationIdentity identity, ConfigurationModel model, object config)
        {
            ConfigInstance instance;
            if (model is ConfigurationOptionModel)
                instance = ConfigFactory.CreateGenericCollectionInstance(model.Type, identity);
            else
                instance = ConfigFactory.CreateGenericInstance(model.Type, identity);
            instance.SetConfiguration(config);
            return instance;
        }
    }
}
