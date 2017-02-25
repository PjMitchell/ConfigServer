using Newtonsoft.Json;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Common settings for text storage
    /// </summary>
    public interface ITextStorageSetting
    {
        /// <summary>
        /// Json Serialization Settings for text storage
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
