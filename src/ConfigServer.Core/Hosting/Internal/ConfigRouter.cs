using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using ConfigServer.Core.Internal;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Core.Internal
{
    internal class ConfigRouter
    {
        readonly IConfigRepository configRepository;
        readonly IEnumerable<ConfigurationModel> configModelCollection;
        readonly IConfigHttpResponseFactory responseFactory;

        public ConfigRouter(IConfigRepository configRepository, IConfigHttpResponseFactory responseFactory, ConfigurationSetRegistry configCollection)
        {
            this.responseFactory = responseFactory;
            this.configModelCollection = configCollection.SelectMany(s=> s.Configs).ToList();
            this.configRepository = configRepository;
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var configSetIds = await configRepository.GetClientsAsync();
            var configSetIdResult = configSetIds.TryMatchPath(c => c.ClientId.ToString(),
            context.Request.Path);
            if(!configSetIdResult.HasResult)
                return false;
            var configModelResult = configModelCollection.TryMatchPath(s => s.Type.Name, configSetIdResult.RemainingPath);
            if (!configModelResult.HasResult)
                return false;
            var config = await configRepository.GetAsync(configModelResult.QueryResult.Type, new ConfigurationIdentity { ClientId = configSetIdResult.QueryResult.ClientId.ToString() });
            await responseFactory.BuildResponse(context, config.GetConfiguration());
            return true;
        }
    }
}
