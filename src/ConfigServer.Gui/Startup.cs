using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConfigServer.Server;
using ConfigServer.FileProvider;
using ConfigServer.AzureBlobStorageProvider;
using ConfigServer.Gui.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.TestModels;
using Microsoft.Extensions.FileProviders;
using System.IO;

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
            services.AddMemoryCache();
            // Add framework services.
            var configserverBuilder = services.AddConfigServer()
                .WithVersion(new Version(1, 0, 0))
                .UseInMemoryCachingStrategy()
                .UseConfigSet<SampleConfigSet>()
                .UseConfigSet<SampleConfigSetRequiringTag>();

            UseFileStorage(configserverBuilder);
            // Comment line above and uncomment line below to test azure blob storage provider
            // UseAzureBlobStorage(configserverBuilder);

            services.AddTransient<IOptionProvider, OptionProvider>();
            services.AddSingleton<UserProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.Use(AddUserContext);
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    context.Context.Response.Headers["Pragma"] = "no-cache";
                    context.Context.Response.Headers["Expires"] = "-1";
                }
            });
            var path = "node_modules";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, path)
                    ),
                RequestPath = "/" + path
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseConfigServer(new ConfigServerOptions());
        }

        private void UseFileStorage(ConfigServerBuilder builder)
        {
            builder.UseFileConfigProvider(new FileConfigRespositoryBuilderOptions { ConfigStorePath = Env.ContentRootPath + "/FileStore/Configs" })
            .UseFileResourceProvider(new FileResourceRepositoryBuilderOptions { ResourceStorePath = Env.ContentRootPath + "/FileStore/Resources" });
        }

        private void UseAzureBlobStorage(ConfigServerBuilder builder)
        {
            builder.UseAzureBlobStorageConfigProvider(new AzureBlobStorageRepositoryBuilderOptions
            {
                Uri = new Uri("http://127.0.0.1:10000/devstoreaccount1"),
                Credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("devstoreaccount1", "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="),
                Container = "configs",
            })
                .UseAzureBlobStorageResourceProvider(new AzureBlobStorageResourceStoreOptions
                {
                    Uri = new Uri("http://127.0.0.1:10000/devstoreaccount1"),
                    Credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("devstoreaccount1", "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="),
                    Container = "resources",
                });
        }

        private Task AddUserContext(HttpContext context, Func<Task> next)
        {
            var userProvider = context.RequestServices.GetService<UserProvider>();
            var pricipal = userProvider.GetPrincipal();
            context.User = pricipal;
            return next();
        }

    }

}
