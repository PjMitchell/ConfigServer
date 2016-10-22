using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConfigServer.Core;
using ConfigServer.Sample.mvc.Models;
using ConfigServer.Server;
using ConfigServer.InMemoryProvider;

namespace ConfigServer.Gui
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var application2Id = "6A302E7D-05E9-4188-9612-4A2920E5C1AE";
            services.AddMvc();
            // Add framework services.
            services.AddConfigServer()
                .UseConfigSet<SampleConfigSet>()
                .UseInMemoryProvider();
            services.AddTransient<IOptionProvider, OptionProvider>();

            var optionProvider = new OptionProvider();
            var config = new SampleConfig
            {
                LlamaCapacity = 23,
                Name = "Name",
                Decimal = 23.47m,
                StartDate = new DateTime(2013, 10, 10),
                IsLlamaFarmer = false,
                Option = optionProvider.GetOptions().First(),
                MoarOptions = optionProvider.GetOptions().Take(2).ToList(),
                ListOfConfigs = new List<ListConfig>
                {
                    new ListConfig { Name = "Value One", Value = 1 },
                    new ListConfig { Name = "Value Two", Value = 2 }
                }
            };
            var config2 = new SampleConfig
            {
                LlamaCapacity = 41,
                Name = "Name 2",
                Decimal = 41.47m,
                StartDate = new DateTime(2013, 11, 11),
                Choice = Choice.OptionThree,
                IsLlamaFarmer = true,
                Option = optionProvider.GetOptions().First(),
                MoarOptions = optionProvider.GetOptions().Take(2).ToList(),
            };
            var serviceProvider = services.BuildServiceProvider();
            var configRepo = serviceProvider.GetService<IConfigRepository>();
            configRepo.UpdateClientAsync(new ConfigurationClient { ClientId = applicationId, Name = "Mvc App", Description = "Embeded Application" }).Wait();
            configRepo.UpdateClientAsync(new ConfigurationClient { ClientId = application2Id, Name = "Mvc App 2", Description = "Second Application" }).Wait();

            configRepo.UpdateConfigAsync(new ConfigInstance<SampleConfig> { ClientId = applicationId, Configuration = config }).Wait();
            configRepo.UpdateConfigAsync(new ConfigInstance<SampleConfig> { ClientId = application2Id, Configuration = config2 }).Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseConfigServer(new ConfigServerOptions {
                ServerAuthenticationOptions = new ConfigServerAuthenticationOptions { RequireAuthentication = false },
                ManagerAuthenticationOptions = new ConfigServerAuthenticationOptions { RequireAuthentication = false }


            });
        }
    }
}
