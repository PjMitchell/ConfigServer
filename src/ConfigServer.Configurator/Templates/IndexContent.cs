using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using ConfigServer.Core;

namespace ConfigServer.Configurator.Templates
{
    internal static class IndexContent
    {
        public static string GetContent(PathString routeString, IEnumerable<ConfigurationClient> configIdentities)
        {
            var configIdentityLinks = configIdentities.Select(s => LinkForConfigSet(s, routeString));
            return $@"
            <H3>Configuration Identity</H3>
            <a href=""{routeString}/Create"">Create New</a>
            <br>
            { string.Join("<br>", configIdentityLinks)}";
        }

        public static string GetContent(PathString routePath, ConfigurationClient client, ConfigurationSetRegistry configSets)
        {
            var configSetLinks = configSets.Select(config => LinkForConfig(client.ClientId, config, routePath));
            return $@"
            <H3>{client.Name} - Config Sets</H3>
            <p>{client.Description}</p>
            <br>
            <a href=""{routePath}/Edit/{client.ClientId}"">Edit {client.Name}</a>
            <br>
            {string.Join("<br>", configSetLinks)}";
        }

        public static string GetContent(PathString routePath, ConfigurationClient client, ConfigurationSetModel configSetDef)
        {
            var configLinks = configSetDef.Configs.Select(config => LinkForConfig(client.ClientId, configSetDef, config, routePath));
            return $@"
            <H3>{client.Name} - {configSetDef.ConfigSetType.Name}</H3>            
            <br>
            {string.Join("<br>", configLinks)}";
        }

        private static string LinkForConfigSet(ConfigurationClient configIdentity, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity.ClientId}\">{configIdentity.Name}</a>";
        }

        private static string LinkForConfig(string configIdentity, ConfigurationSetModel config, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity}/{config.ConfigSetType.Name}\">{config.ConfigSetType.Name}</a>";
        }
        private static string LinkForConfig(string configIdentity, ConfigurationSetModel configSet, ConfigurationModel configModel, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity}/{configSet.ConfigSetType.Name}/{configModel.Type.Name}\">{configModel.ConfigurationDisplayName}</a>";
        }

    }
}
