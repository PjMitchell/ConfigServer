using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal static class HttpContextExtensions
    {
        public static async Task<JObject> GetJObjectFromJsonBodyAsync(this HttpContext source)
        {
            var json = await source.ReadBodyTextAsync();
            var result = JObject.Parse(json);
            return result;
        }

        public static async Task<T> GetObjectFromJsonBodyAsync<T>(this HttpContext source)
        {
            var json = await source.ReadBodyTextAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        private static Task<string> ReadBodyTextAsync(this HttpContext source)
        {
            var body = source.Request.Body;
            return new StreamReader(body).ReadToEndAsync();
        }
    }
}
