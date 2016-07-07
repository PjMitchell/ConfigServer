using ConfigServer.Configurator.Templates;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core.Internal;

namespace ConfigServer.Configurator
{
    internal class ConfiguratorRouter
    {
        private readonly IConfigRepository configRepository;
        private readonly ConfigurationSetCollection configCollection;
        private readonly PageBuilder pageBuilder;

        internal ConfiguratorRouter(IConfigRepository configRepository, ConfigurationSetCollection configCollection, PageBuilder pageBuilder)
        {
            this.configRepository = configRepository;
            this.configCollection = configCollection;
            this.pageBuilder = pageBuilder;
        }

        public async Task<bool> HandleRequest(HttpContext context, PathString routePath)
        {
            PathString remaining;
            if (!context.Request.Path.StartsWithSegments(routePath, out remaining))
                return false;
            
            var applicationIds = await configRepository.GetConfigSetIdsAsync();

            if (string.IsNullOrWhiteSpace(remaining))
            {
                await pageBuilder.WriteContent("Index", IndexContent.GetContent(routePath, applicationIds));
                return true;
            }

            if(remaining == "/Create")
            {
                return await HandleCreateConfigSet(context, routePath);
            }
                
            var appIdQueryResult = ContainsElement(applicationIds, remaining);
            if (!appIdQueryResult.HasResult)
                return false;
            var appId = appIdQueryResult.QueryResult;
            var configs = configCollection;
            if(string.IsNullOrEmpty(appIdQueryResult.RemainingPath))
            {
                await pageBuilder.WriteContent("Index", IndexContent.GetContent(routePath, appId, configs));
                return true; //Lists ConfigSet
            }
            
            var configSetQueryResult = configs.TryMatchPath(s=> s.ConfigSetType.Name, appIdQueryResult.RemainingPath);
            if (!configSetQueryResult.HasResult)
                return false;
            var configSetDefinition = configSetQueryResult.QueryResult;
            var configQueryResult = configSetQueryResult.QueryResult.Configs.TryMatchPath(s => s.Type.Name, configSetQueryResult.RemainingPath);
            if (!configQueryResult.HasResult)
            {
                await pageBuilder.WriteContent(configSetQueryResult.QueryResult.ConfigSetType.Name, IndexContent.GetContent(routePath, appId, configSetDefinition));
                return true; //Lists ConfigSet Configs
            }

            var currentConfig =await configRepository.GetAsync(configQueryResult.QueryResult.Type, new ConfigurationIdentity { ConfigSetId = appIdQueryResult.QueryResult });

            if (context.Request.Method.Equals("GET"))
            {
                await pageBuilder.WriteContent(configQueryResult.QueryResult.Type.Name, EditorContent.GetContent(currentConfig, configQueryResult.QueryResult));
                return true; 
            }

            if (context.Request.Method.Equals("POST"))
            {
                var configResult = ConfigFormBinder.BindForm(currentConfig, context.Request.Form);
                await configRepository.SaveChangesAsync(configResult);
                pageBuilder.Redirect(routePath + "/" + appIdQueryResult.QueryResult);
                return true;
            }

            return false;
        }

        private async Task<bool> HandleCreateConfigSet(HttpContext context, PathString routePath)
        {
            if (context.Request.Method.Equals("GET"))
            {
                await pageBuilder.WriteContent("Create ConfigSet", CreateConfigSetContent.GetContent());
                return true;
            }

            if (context.Request.Method.Equals("POST"))
            {
                var configSetId = CreateConfigSetFormBinder.BindForm(context.Request.Form);
                await configRepository.CreateConfigSetAsync(configSetId);
                pageBuilder.Redirect(routePath);
                return true;
            }

            return false;
        }

        private PathQueryResult<string> ContainsElement(IEnumerable<string> paths, PathString pathToQuery)
        {
            return paths.TryMatchPath(r => r, pathToQuery);
        }

       
    }
}
