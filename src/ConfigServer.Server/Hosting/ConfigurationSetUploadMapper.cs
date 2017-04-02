using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetUploadMapper
    {
        IEnumerable<KeyValuePair<string,object>> MapConfigurationSetUpload(JObject upload, ConfigurationSetModel model);
    }

    internal class ConfigurationSetUploadMapper : IConfigurationSetUploadMapper
    {
        public IEnumerable<KeyValuePair<string, object>> MapConfigurationSetUpload(JObject upload, ConfigurationSetModel model)
        {
            foreach (var item in model.Configs.Where(w => !w.IsReadOnly))
            {
                if (!upload.TryGetValue(item.Name,StringComparison.OrdinalIgnoreCase, out var configJToken))
                    continue;
                var config = ToObjectOrDefault(configJToken, item);
                yield return new KeyValuePair<string, object>(item.Name, config);
            }
        }

        private object ToObjectOrDefault(JToken token, ConfigurationModel model)
        {
            if (model is ConfigurationOptionModel option)
                return ToObjectOrDefault(token, option.StoredType);
            return ToObjectOrDefault(token, model.Type);
        }

        private object ToObjectOrDefault(JToken token, Type type)
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

    }
}
