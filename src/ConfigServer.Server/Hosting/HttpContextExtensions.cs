using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
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

        public static async Task<object> GetObjectFromJsonBodyOrDefaultAsync(this HttpContext source, Type type)
        {
            var json = await source.ReadBodyTextAsync();
            bool failed = false;
            var result = JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
            {
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    failed = true;
                    args.ErrorContext.Handled = true;
                }
            });
            return failed? null: result;
        }

        private static Task<string> ReadBodyTextAsync(this HttpContext source)
        {
            var body = source.Request.Body;
            return new StreamReader(body).ReadToEndAsync();
        }
    }
}
