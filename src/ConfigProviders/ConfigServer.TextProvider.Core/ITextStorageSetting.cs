using Newtonsoft.Json;

namespace ConfigServer.TextProvider.Core
{
    public interface ITextStorageSetting
    {
        JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
