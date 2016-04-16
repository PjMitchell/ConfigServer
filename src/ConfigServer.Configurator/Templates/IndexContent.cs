using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigServer.Core;

namespace ConfigServer.Configurator.Templates
{
    public static class IndexContent
    {
        public static string GetContent(PathString routeString, IEnumerable<string> configSetIds)
        {
            var configSetLinks = configSetIds.Select(s => LinkForConfigSet(s, routeString));
            return $@"
            <H3>Configuration Sets</H3>
            <a href=""{routeString}/Create"">Create New</a>
            <br>
            { string.Join("<br>", configSetLinks)}";
        }

        public static string GetContent(PathString routePath, string configSetId, List<ConfigurationRegistration> configs)
        {
            var configLinks = configs.Select(config => LinkForConfig(configSetId, config, routePath));
            return $@"
            <H3>{configSetId} - Configs</H3>
            {string.Join("<br>", configLinks)}";
        }

        private static string LinkForConfigSet(string configSetId, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configSetId}\">{configSetId}</a>";
        }

        private static string LinkForConfig(string configSetId, ConfigurationRegistration config, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configSetId}/{config.ConfigurationName}\">{config.ConfigurationName}</a>";
        }

        
    }
}
