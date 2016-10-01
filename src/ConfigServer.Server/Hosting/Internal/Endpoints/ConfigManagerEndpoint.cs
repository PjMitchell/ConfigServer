using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigManagerEndpoint : IEndpoint
    {
        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.AuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var managerPath = context.Request.PathBase.Value;
            var basePath = GetBasePath(managerPath);

            await context.Response.WriteAsync(Shell(basePath, managerPath));
            return true;
        }

        private string GetBasePath(string managerPath)
        {
            var basePath = managerPath;
            var desiredLength = basePath.Length - HostPaths.Manager.Length;
            var trimmedBasePath = basePath.Substring(0, desiredLength);
            return trimmedBasePath;
        }

        private string Shell(string basePath, string managerPath) => $@"
            <html>
            <head>
                <title>Configuration manager</title>
                <meta charset=""UTF-8"">
                <meta name = ""viewport"" content=""width=device-width, initial-scale=1"">
                <link rel = ""stylesheet"" href=""{basePath}/Assets/styles.css"">
                <!-- 1. Load libraries -->
                <!-- Polyfill(s) for older browsers -->
                <script src = ""https://unpkg.com/core-js/client/shim.min.js"" ></script>
                <script src=""https://unpkg.com/zone.js@0.6.25?main=browser""></script>
                <script src = ""https://unpkg.com/reflect-metadata@0.1.3""></script>
                <script src=""https://unpkg.com/systemjs@0.19.27/dist/system.src.js""></script>
                <!-- 2. Configure SystemJS -->
                <script>
                {Config(basePath)}
                </script>
                <script>
                  System.import('app').catch(function(err) {{ console.error(err); }});
                </script>
                <base href=""{managerPath}"" />
            </head>
            <!-- 3. Display the application -->
            <body>
                <config-server-shell>Loading...</config-server-shell>
            </body>
            </html>
            ";
        private string Config(string basePath) => $@"
            (function(global) {{
                System.config({{
        
                    paths: {{
                        // paths serve as alias
                        'npm:': 'https://unpkg.com/'
                    }},
                    map: {{
                        app: '{basePath}/Assets/app',

                        // angular bundles
                        '@angular/core': 'npm:@angular/core/bundles/core.umd.js',
                        '@angular/common': 'npm:@angular/common/bundles/common.umd.js',
                        '@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
                        '@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
                        '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
                        '@angular/http': 'npm:@angular/http/bundles/http.umd.js',
                        '@angular/router': 'npm:@angular/router/bundles/router.umd.js',
                        '@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',
                        '@angular/upgrade': 'npm:@angular/upgrade/bundles/upgrade.umd.js',

                        // other libraries
                        'rxjs': 'npm:rxjs',
                        'angular2-in-memory-web-api': 'npm:angular2-in-memory-web-api'

                    }},
                    // packages tells the System loader how to load when no filename and/or no extension
                    packages: {{
                        app: {{
                            main: './main.js',
                            defaultExtension: 'js'
                        }}
                    }}
                }});
            }})(this);";
    }
}
