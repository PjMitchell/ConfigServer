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
        public static string GetContent(PathString routeString, IEnumerable<string> applicationIds)
        {
            var applicationLinks = applicationIds.Select(s => LinkForApplicationId(s, routeString));
            return $@"
            <H3>Applications</H3>
            {string.Join("<br>", applicationLinks)}";
        }

        public static string GetContent(PathString routePath, string applicationId, List<ConfigurationRegistration> configs)
        {
            var configLinks = configs.Select(config => LinkForConfig(applicationId, config, routePath));
            return $@"
            <H3>{applicationId} - Configs</H3>
            {string.Join("<br>", configLinks)}";
        }

        private static string LinkForApplicationId(string applicationId, PathString routeString)
        {
            return $"<a href=\"{routeString}/{applicationId}\">{applicationId}</a>";
        }

        private static string LinkForConfig(string applicationId, ConfigurationRegistration config, PathString routeString)
        {
            return $"<a href=\"{routeString}/{applicationId}/{config.ConfigurationName}\">{config.ConfigurationName}</a>";
        }

        
    }
}
