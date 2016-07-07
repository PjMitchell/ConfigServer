using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using ConfigServer.Core;

namespace ConfigServer.Configurator.Templates
{
    internal static class IndexContent
    {
        public static string GetContent(PathString routeString, IEnumerable<string> configIdentities)
        {
            var configIdentityLinks = configIdentities.Select(s => LinkForConfigSet(s, routeString));
            return $@"
            <H3>Configuration Identity</H3>
            <a href=""{routeString}/Create"">Create New</a>
            <br>
            { string.Join("<br>", configIdentityLinks)}";
        }

        public static string GetContent(PathString routePath, string configIdentity, ConfigurationSetCollection configSets)
        {
            var configSetLinks = configSets.Select(config => LinkForConfig(configIdentity, config, routePath));
            return $@"
            <H3>{configIdentity} - Config Sets</H3>
            {string.Join("<br>", configSetLinks)}";
        }

        public static string GetContent(PathString routePath, string configIdentity, ConfigurationSetDefinition configSetDef)
        {
            var configLinks = configSetDef.Configs.Select(config => LinkForConfig(configIdentity, configSetDef, config, routePath));
            return $@"
            <H3>{configIdentity} - {configSetDef.ConfigSetType.Name}</H3>
            {string.Join("<br>", configLinks)}";
        }

        private static string LinkForConfigSet(string configIdentity, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity}\">{configIdentity}</a>";
        }

        private static string LinkForConfig(string configIdentity, ConfigurationSetDefinition config, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity}/{config.ConfigSetType.Name}\">{config.ConfigSetType.Name}</a>";
        }
        private static string LinkForConfig(string configIdentity, ConfigurationSetDefinition configSet, ConfigurationModelDefinition configModel, PathString routeString)
        {
            return $"<a href=\"{routeString}/{configIdentity}/{configSet.ConfigSetType.Name}/{configModel.Type.Name}\">{configModel.ConfigurationDisplayName}</a>";
        }

    }
}
