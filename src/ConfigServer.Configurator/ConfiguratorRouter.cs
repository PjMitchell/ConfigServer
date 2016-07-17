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
        private readonly ConfigurationSetRegistry configCollection;
        private readonly PageBuilder pageBuilder;

        internal ConfiguratorRouter(IConfigRepository configRepository, ConfigurationSetRegistry configCollection, PageBuilder pageBuilder)
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
            
            var clients = await configRepository.GetClientsAsync();

            if (string.IsNullOrWhiteSpace(remaining))
            {
                await pageBuilder.WriteContent("Index", IndexContent.GetContent(routePath, clients));
                return true;
            }

            if(remaining == "/Create")
            {
                return await HandleCreateConfigurationClient(context, routePath);
            }

            PathString editRemaining;
            if (remaining.StartsWithSegments("/Edit", out editRemaining))
            {
                var clientResult = clients.TryMatchPath(s => s.ClientId, editRemaining);
                if (!clientResult.HasResult)
                    return false;
                if (context.Request.Method.Equals("GET"))
                {
                    await pageBuilder.WriteContent($"Edit {clientResult.QueryResult.Name}", EditClientContent.GetContent(clientResult.QueryResult));
                    return true;
                }

                if (context.Request.Method.Equals("POST"))
                {
                    var result = UpdateConfigurationClientFormBinder.BindForm(context.Request.Form);
                    await configRepository.UpdateClientAsync(result);
                    pageBuilder.Redirect(routePath + "/" + clientResult.QueryResult.ClientId);
                    return true;
                }
                return false;
            }

            var clientQueryResult = clients.TryMatchPath(s=> s.ClientId, remaining);
            if (!clientQueryResult.HasResult)
                return false;
            var appId = clientQueryResult.QueryResult;
            var configs = configCollection;
            if(string.IsNullOrEmpty(clientQueryResult.RemainingPath))
            {
                await pageBuilder.WriteContent("Index", IndexContent.GetContent(routePath, appId, configs));
                return true; //Lists ConfigSet
            }
            
            var configSetQueryResult = configs.TryMatchPath(s=> s.ConfigSetType.Name, clientQueryResult.RemainingPath);
            if (!configSetQueryResult.HasResult)
                return false;
            var configSetDefinition = configSetQueryResult.QueryResult;
            var configQueryResult = configSetQueryResult.QueryResult.Configs.TryMatchPath(s => s.Type.Name, configSetQueryResult.RemainingPath);
            if (!configQueryResult.HasResult)
            {
                await pageBuilder.WriteContent(configSetQueryResult.QueryResult.ConfigSetType.Name, IndexContent.GetContent(routePath, appId, configSetDefinition));
                return true; //Lists ConfigSet Configs
            }

            var currentConfig =await configRepository.GetAsync(configQueryResult.QueryResult.Type, new ConfigurationIdentity { ClientId = clientQueryResult.QueryResult.ClientId });

            if (context.Request.Method.Equals("GET"))
            {
                await pageBuilder.WriteContent(configQueryResult.QueryResult.Type.Name, EditorContent.GetContent(clientQueryResult.QueryResult, currentConfig, configQueryResult.QueryResult));
                return true; 
            }

            if (context.Request.Method.Equals("POST"))
            {
                var configResult = ConfigFormBinder.BindForm(currentConfig, context.Request.Form);
                await configRepository.UpdateConfigAsync(configResult);
                pageBuilder.Redirect(routePath + "/" + clientQueryResult.QueryResult);
                return true;
            }

            return false;
        }

        private async Task<bool> HandleCreateConfigurationClient(HttpContext context, PathString routePath)
        {
            if (context.Request.Method.Equals("GET"))
            {
                await pageBuilder.WriteContent("Create ConfigSet", CreateClientContent.GetContent());
                return true;
            }

            if (context.Request.Method.Equals("POST"))
            {
                var client = UpdateConfigurationClientFormBinder.BindForm(context.Request.Form);
                await configRepository.UpdateClientAsync(client);
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
