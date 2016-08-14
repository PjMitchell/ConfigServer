using Newtonsoft.Json;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// Options for FileConfigRespository
    /// </summary>
    public class FileConfigRespositoryBuilderOptions
    {
        /// <summary>
        /// Path to config store folder
        /// </summary>
        public string ConfigStorePath { get; set; }

        /// <summary>
        /// Settings for config JsonSerializer
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();
    }
}
