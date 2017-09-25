using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConfigServer.Sample.mvc.Models;
using ConfigServer.Core;
using ConfigServer.Server;
using ConfigServer.FileProvider;
using System.Linq;
using System.Collections.Generic;
using ConfigServer.Client.Builder;

namespace ConfigServer.Sample.mvc
{
    public class Startup
    {
        private readonly IHostingEnvironment enviroment;
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            enviroment = env;
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            //var application2Id = "6A302E7D-05E9-4188-9612-4A2920E5C1AE";
            //var groupId = "6C3E9253-8DB9-4C7D-AAFC-12391CB7B1C8";
            // Add framework services.
            services.AddMvc();
            services.AddMemoryCache();
            services.AddConfigServer()
                .WithVersion(new Version(1,0,0))
                .UseInMemoryCachingStrategy()
                .UseConfigSet<SampleConfigSet>()
                .UseFileConfigProvider(new FileConfigRespositoryBuilderOptions { ConfigStorePath = enviroment.ContentRootPath + "/FileStore/Configs" })
                .UseFileResourceProvider(new FileResourceRepositoryBuilderOptions { ResourceStorePath = enviroment.ContentRootPath + "/FileStore/Resources" })
                .UseLocalConfigServerClient(new Uri("http://localhost:58201/Config"))
                .WithClientId(applicationId)
                .WithConfig<SampleConfig>()
                .WithCollectionConfig<OptionFromConfigSet>();

            services.AddTransient<IOptionProvider, OptionProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            

            app.UseStaticFiles();
            app.Map("/Config", configSrv => configSrv.UseConfigServer(new ConfigServerOptions {
                ClientAdminClaimType = string.Empty,
                ClientConfiguratorClaimType = string.Empty,
                ClientReadClaimType = string.Empty,
                AllowAnomynousAccess = true

            }));
            app.Map("/Resource", innerApp => innerApp.UseResourceServer());
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
