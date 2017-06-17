using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetUploadMapper
    {
        IEnumerable<KeyValuePair<string,object>> MapConfigurationSetUpload(string upload, ConfigurationSetModel model);
    }

    internal class ConfigurationSetUploadMapper : IConfigurationSetUploadMapper
    {
        private readonly IConfigurationUploadMapper configurationUploadMapper;

        public ConfigurationSetUploadMapper(IConfigurationUploadMapper configurationUploadMapper)
        {
            this.configurationUploadMapper = configurationUploadMapper;
        }

        public IEnumerable<KeyValuePair<string, object>> MapConfigurationSetUpload(string upload, ConfigurationSetModel model)
        {
            var jObject = JObject.Parse(upload); 
            foreach (var item in model.Configs.Where(w => !w.IsReadOnly))
            {
                if (!jObject.TryGetValue(item.Name,StringComparison.OrdinalIgnoreCase, out var configJToken))
                    continue;
                var config = configurationUploadMapper.ToObjectOrDefault(configJToken.ToString(), item);
                yield return new KeyValuePair<string, object>(item.Name, config);
            }
        }


    }
}
