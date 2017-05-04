using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConfigServer.Server;
using ConfigServer.FileProvider;
using ConfigServer.Gui.Models;
using System;

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
            Env = env;
        }

        public IHostingEnvironment Env { get; }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Add framework services.
            services.AddConfigServer()
                .WithVersion(new Version(1, 0, 0))
                .UseConfigSet<SampleConfigSet>()
                .UseFileConfigProvider(new FileConfigRespositoryBuilderOptions { ConfigStorePath = Env.ContentRootPath +"/FileStore/Configs" })
                .UseFileResourceProvider(new FileResourceRepositoryBuilderOptions { ResourceStorePath = Env.ContentRootPath + "/FileStore/Resources" });
            services.AddTransient<IOptionProvider, OptionProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    context.Context.Response.Headers["Pragma"] = "no-cache";
                    context.Context.Response.Headers["Expires"] = "-1";
                }
            });

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
