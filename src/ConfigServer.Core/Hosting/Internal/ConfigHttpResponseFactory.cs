using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace ConfigServer.Core.Internal
{
    /// <summary>
    /// Serializes config object then adds it to http context response
    /// </summary>
    public interface IConfigHttpResponseFactory
    {
        /// <summary>
        /// Builds config object then adds it to http context 
        /// </summary>
        /// <param name="context">HttpContext of request</param>
        /// <param name="config">Config to be serialized</param>
        /// <returns>A task that represents the asynchronous build operation.</returns>
        Task BuildResponse(HttpContext context, object config);
    }

    internal class ConfigHttpResponseFactory : IConfigHttpResponseFactory
    {
        public Task BuildResponse(HttpContext context, object config)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(config, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
}
