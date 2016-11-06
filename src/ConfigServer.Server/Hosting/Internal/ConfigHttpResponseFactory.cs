using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ConfigServer.Server
{
    /// <summary>
    /// Serializes config object then adds it to http context response
    /// </summary>
    internal interface IConfigHttpResponseFactory
    {
        /// <summary>
        /// Builds config object then adds it to http context 
        /// </summary>
        /// <param name="context">HttpContext of request</param>
        /// <param name="config">ConfigInstance to be serialized</param>
        /// <returns>A task that represents the asynchronous build operation.</returns>
        Task BuildResponse(HttpContext context, object config);

        void BuildNoContentResponse(HttpContext context);
        Task BuildJsonFileResponse(HttpContext context, object config, string fileName);
    }

    internal class ConfigHttpResponseFactory : IConfigHttpResponseFactory
    {
        public void BuildNoContentResponse(HttpContext context)
        {
            context.Response.StatusCode = 204;
        }

        public Task BuildResponse(HttpContext context, object config)
        {
            context.Response.ContentType = HttpContentType.Json;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(config, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        public Task BuildJsonFileResponse(HttpContext context, object config,string fileName)
        {
            context.Response.ContentType = HttpContentType.Json;
            context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return context.Response.WriteAsync(JsonConvert.SerializeObject(config, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }

    internal class HttpContentType
    {
        public const string Json = "application/json";
    }
}
