using ConfigServer.FileProvider;
using ConfigServer.Server;
using ConfigServer.TestModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConfigServer.IntegrationTests
{
    public class FileStartUp
    {
        public const string username = "name";
        public const string role = "role";
        public const string auth = "hkd";

        public IHostingEnvironment Env { get; }
        public IConfigurationRoot Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMemoryCache();
                // Add framework services.
                var configserverBuilder = services.AddConfigServer()
                    .WithVersion(new Version(1, 0, 0))
                    .UseInMemoryCachingStrategy()
                    .UseConfigSet<SampleConfigSet>();

                UseFileStorage(configserverBuilder);
                // Comment line above and uncomment line below to test azure blob storage provider
                // UseAzureBlobStorage(configserverBuilder);

                services.AddTransient<IOptionProvider, OptionProvider>();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.Use(AddUserContext);
                app.UseConfigServer(new ConfigServerOptions());
            }

            private void UseFileStorage(ConfigServerBuilder builder)
            {
                builder.UseFileConfigProvider(new FileConfigRespositoryBuilderOptions { ConfigStorePath = @"./SeedData/Configs" })
                .UseFileResourceProvider(new FileResourceRepositoryBuilderOptions { ResourceStorePath = @"./SeedData/Resources" });
            }

            private Task AddUserContext(HttpContext context, Func<Task> next)
            {
                var claims = new List<Claim>
                {
                    new Claim(username, "A.Person"),
                    new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue)
                };
                var identity = new ClaimsIdentity(claims, auth, username, role);
                    var principal = new ClaimsPrincipal(identity);
                    context.User = principal;
                    return next();
                }

        }
    }
