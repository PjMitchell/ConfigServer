using Newtonsoft.Json;

namespace ConfigServer.FileProvider
{
    public class FileConfigRespositoryBuilderOptions
    {
        public string ConfigStorePath { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
