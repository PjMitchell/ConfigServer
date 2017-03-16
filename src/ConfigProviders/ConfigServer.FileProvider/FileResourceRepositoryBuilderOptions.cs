using Newtonsoft.Json;
using ConfigServer.TextProvider.Core;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// Options for FileConfigRespository
    /// </summary>
    public class FileResourceRepositoryBuilderOptions
    {
        /// <summary>
        /// Path to config store folder
        /// </summary>
        public string ResourceStorePath { get; set; }
    }
}
