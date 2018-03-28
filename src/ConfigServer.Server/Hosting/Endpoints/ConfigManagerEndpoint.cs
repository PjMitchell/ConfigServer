using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigManagerEndpoint : IEndpoint
    {
        private readonly IHttpResponseFactory httpResponseFactory;

        public ConfigManagerEndpoint(IHttpResponseFactory httpResponseFactory)
        {
            this.httpResponseFactory = httpResponseFactory;
        }

        public Task Handle(HttpContext context, ConfigServerOptions options)
        {
            var managerPath = context.Request.PathBase.Value;
            var basePath = GetBasePath(managerPath);
            if (!CheckMethodAndAuthentication(context, options))
                return Task.FromResult(true);
            return context.Response.WriteAsync(Shell(basePath, managerPath, options));
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }

        private string GetBasePath(string managerPath)
        {
            var basePath = managerPath;
            var desiredLength = basePath.Length - HostPaths.Manager.Length;
            var trimmedBasePath = basePath.Substring(0, desiredLength);
            return trimmedBasePath;
        }

        private string Shell(string basePath, string managerPath, ConfigServerOptions options) => $@"
            <html>
            <head>
                <title>Configuration manager</title>
                <meta charset=""UTF-8"">
                <meta name = ""viewport"" content=""width=device-width, initial-scale=1"">
                <link href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" rel=""stylesheet"" />
                <link href=""{GetThemeUrl(basePath, options)}"" rel=""stylesheet"" />
                <link rel = ""stylesheet"" href=""{basePath}/Assets/styles.css"">
                <!-- 1. Load libraries -->
                <!-- Polyfill(s) for older browsers -->
                <script src=""{basePath}/Assets/lib/shim.min.js""></script>
                <script src = ""{basePath}/Assets/lib/zone.min.js"" ></script>
                <script src=""{basePath}/Assets/lib/system.js""></script>
                <!-- 2. Configure SystemJS -->
                <script>
                  System.import('{basePath}/Assets/app.js').catch(function(err) {{ console.error(err); }});
                </script>
                <base href=""{managerPath}"" />
            </head>
            <!-- 3. Display the application -->
            <body>
                <config-server-shell>Loading...</config-server-shell>
            </body>
            </html>
            ";
        private static string GetThemeUrl(string basePath, ConfigServerOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ThemeUrl))
                return $"{basePath}/Assets/lib/deeppurple-amber.css";
            return options.ThemeUrl;
        }
    }
}
