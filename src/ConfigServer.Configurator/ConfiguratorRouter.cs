using ConfigServer.Configurator.Templates;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigServer.Core;

namespace ConfigServer.Configurator
{
    public class ConfiguratorRouter
    {
        private readonly IConfigRepository configRepository;
        private readonly ConfigurationCollection configCollection;
        private readonly PageBuilder pageBuilder;
        public ConfiguratorRouter(IConfigRepository configRepository, ConfigurationCollection configCollection, PageBuilder pageBuilder)
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
            
            var applicationIds = configRepository.GetConfigSetIds();

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
            var configs = configCollection.ToList();
            if(string.IsNullOrEmpty(appIdQueryResult.RemainingPath))
            {
                await pageBuilder.WriteContent("Index", IndexContent.GetContent(routePath, appIdQueryResult.QueryResult, configs));
                return true;
            }
            
            var configQueryResult = ContainsElement(configs,s=> s.ConfigurationName, appIdQueryResult.RemainingPath);
            if(!configQueryResult.HasResult)
            {
                await pageBuilder.WriteContent(configQueryResult.QueryResult.ConfigurationName, IndexContent.GetContent(routePath, appIdQueryResult.QueryResult, configs));
                return true;
            }

            var currentConfig =await configRepository.GetAsync(configQueryResult.QueryResult.ConfigType, new ConfigurationIdentity { ConfigSetId = appIdQueryResult.QueryResult });

            if (context.Request.Method.Equals("GET"))
            {
                await pageBuilder.WriteContent(configQueryResult.QueryResult.ConfigurationName, EditorContent.GetContent(currentConfig));
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

        private PathQueryResult<T> ContainsElement<T>(IEnumerable<T>paths,Func<T,string> selector,  PathString pathToQuery)
        {
            foreach(var path in paths)
            {
                var queryPath = $"/{selector(path)}";
                PathString remainingPath;
                if (pathToQuery.StartsWithSegments(queryPath, out remainingPath))
                    return PathQueryResult<T>.Success(path, remainingPath);             
            }
            return PathQueryResult<T>.Failed<T>();
        }

        private PathQueryResult<string> ContainsElement(IEnumerable<string> paths, PathString pathToQuery)
        {
            return ContainsElement<string>(paths, r => r, pathToQuery);
        }

        private class PathQueryResult<T>
        {
            private PathQueryResult()
            {

            }

            public static PathQueryResult<TResult> Success<TResult>(TResult queryResult, PathString remainingPath)
            {
                return new PathQueryResult<TResult>
                {
                    HasResult = true,
                    QueryResult = queryResult,
                    RemainingPath = remainingPath
                };
            }

            public static PathQueryResult<TResult> Failed<TResult>()
            {
                return new PathQueryResult<TResult>();
            }

            public bool HasResult { get; private set; }
            public T QueryResult { get; private set; }
            public PathString RemainingPath { get; private set; }
        }

    }
}
