using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConfigServer.Infrastructure;
using ConfigServer.InMemoryProvider;
using ConfigServer.Sample.mvc.Models;
using ConfigServer.Core;
using ConfigServer.Configurator;
using ConfigServer.Core.Hosting;


namespace ConfigServer.Sample.mvc
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            // Add framework services.
            services.AddMvc();
            services.UseConfigServer()
                .UseConfigSet<SampleConfigSet>()
                .UseInMemoryProvider()
                .UseLocalConfigServerClient(applicationId)
                .WithConfig<SampleConfig>();
            var config = new SampleConfig
            {
                LlamaCapacity = 23,
                Name = "Name",
                Decimal = 23.47m,
                StartDate = new DateTime(2013,10,10),
                IsLlamaFarmer = false
            };
            var serviceProvider = services.BuildServiceProvider();
            var configRepo = serviceProvider.GetService<IConfigRepository>();
            configRepo.SaveChanges(new Config<SampleConfig> { ConfigSetId = applicationId, Configuration = config });
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
            app.Map("/Config", configSrv => configSrv.UseConfigServer());
            app.UseConfigServerConfigurator();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
