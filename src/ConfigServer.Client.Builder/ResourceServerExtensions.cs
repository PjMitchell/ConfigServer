using System;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;

namespace ConfigServer.Client.Builder
{
    /// <summary>
    /// Extension for wiring up Resource server
    /// </summary>
    public static class ResourceServerExtensions
    {
        /// <summary>
        /// Serves up Resources from configserver at /{resourcename}
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseResourceServer(this IApplicationBuilder app) 
        {
            return app.Use(ServeResource);
        }

        private static async Task ServeResource(HttpContext context, Func<Task> next) 
        {
            var pathParams = context.Request.Path.Value.Split('/').Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();
            if(pathParams.Length != 1)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                var serviceProvider = context.RequestServices;
                var server = serviceProvider.GetRequiredService<IConfigServer>();
                var response = await server.GetResourceAsync(pathParams[0]);
                if (response.HasEntry)
                    await BuildFileResponse(context, response.Content, response.Name);
                else
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }

        private static Task BuildFileResponse(HttpContext context, Stream file, string fileName)
        {
            context.Response.ContentType = "application/octet-stream";
            context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            context.Response.RegisterForDispose(file);
            return file.CopyToAsync(context.Response.Body);
        }
    }
}
