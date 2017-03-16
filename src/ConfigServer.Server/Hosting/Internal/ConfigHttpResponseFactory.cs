using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

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

        void BuildStatusResponse(HttpContext context, int statusCode);
        Task BuildFileResponse(HttpContext context, Stream file, string fileName);

        Task BuildJsonFileResponse(HttpContext context, object config, string fileName);

        Task BuildInvalidRequestResponse(HttpContext context, IEnumerable<string> errors);
    }

    internal class ConfigHttpResponseFactory : IConfigHttpResponseFactory
    {
        public void BuildNoContentResponse(HttpContext context) => BuildStatusResponse(context, 204);

        public void BuildStatusResponse(HttpContext context, int statusCode)
        {
            context.Response.StatusCode = statusCode;
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

        public Task BuildInvalidRequestResponse(HttpContext context, IEnumerable<string> errors)
        {
            context.Response.StatusCode = 409;
            context.Response.ContentType = HttpContentType.Text;
            var body = string.Join(Environment.NewLine, errors);
            return context.Response.WriteAsync(body);
        }

        public Task BuildFileResponse(HttpContext context, Stream file, string fileName)
        {
            context.Response.ContentType = HttpContentType.Stream;
            context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            context.Response.RegisterForDispose(file);
            return file.CopyToAsync(context.Response.Body);
        }
    }

    internal class HttpContentType
    {
        public const string Json = "application/json";
        public const string Text = "text/plain";
        public const string Stream = "application/octet-stream";
    }
}
