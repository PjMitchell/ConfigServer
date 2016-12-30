using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class UploadEnpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigurationValidator confgiurationValidator;
        readonly IConfigurationSetUploadMapper configurationSetUploadMapper;

        public UploadEnpoint(IConfigHttpResponseFactory responseFactory, IConfigInstanceRouter configInstanceRouter, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator confgiurationValidator, IConfigurationSetUploadMapper configurationSetUploadMapper)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.configInstanceRouter = configInstanceRouter;
            this.configRepository = configRepository;
            this.confgiurationValidator = confgiurationValidator;
            this.configurationSetUploadMapper = configurationSetUploadMapper;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var routePath = context.Request.Path;
            if (context.Request.Method != "POST")
                return false;
            PathString remainingPath;
            if (routePath.StartsWithSegments("/Configuration", out remainingPath))
            {
                var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(remainingPath);
                if (configInstance == null)
                    return false;
                await HandleUploadRequest(context, configInstance);
                return true;
            }
            if (routePath.StartsWithSegments("/ConfigurationSet", out remainingPath))
            {
                var clients = await configRepository.GetClientsAsync();
                var clientsResult = clients.TryMatchPath(c => c.ClientId, remainingPath);
                if (!clientsResult.HasResult)
                    return false;
                var configSetResult = configCollection.TryMatchPath(c=> c.ConfigSetType.Name,clientsResult.RemainingPath);
                if (!configSetResult.HasResult)
                    return false;
                await HandleUploadRequest(context, clientsResult.QueryResult.ClientId, configSetResult.QueryResult);
                return true;
            }
            return false;
        }
        private async Task HandleUploadRequest(HttpContext context, ConfigInstance configInstance)
        {
            var input = await context.GetObjectFromJsonBodyOrDefaultAsync(configInstance.ConfigType);
            var validationResult = confgiurationValidator.Validate(input, GetConfigurationSetForModel(configInstance).Configs.Single(s=> s.Type == configInstance.ConfigType));
            if (validationResult.IsValid)
            {
                configInstance.SetConfiguration(input);
                await configRepository.UpdateConfigAsync(configInstance);
                responseFactory.BuildNoContentResponse(context);
            }
            else
            {
                responseFactory.BuildStatusResponse(context, 422);
            }
        }

        private async Task HandleUploadRequest(HttpContext context,string clientid, ConfigurationSetModel configSetModel)
        {
            var input = await context.GetJObjectFromJsonBodyAsync();
            var mappedConfigs = configurationSetUploadMapper.MapConfigurationSetUpload(input, configSetModel).ToArray();
            var validationResult = ValidateConfigs(mappedConfigs, configSetModel);
            if (validationResult.IsValid)
            {
                foreach(var config in mappedConfigs)
                {
                    var instance = await configRepository.GetAsync(config.Value.GetType(), new ConfigurationIdentity { ClientId = clientid });
                    instance.SetConfiguration(config.Value);
                    await configRepository.UpdateConfigAsync(instance);
                }
                responseFactory.BuildNoContentResponse(context);
            }
            else
            {
                responseFactory.BuildStatusResponse(context, 422);
            }
        }

        private ValidationResult ValidateConfigs(IEnumerable<KeyValuePair<string,object>> source, ConfigurationSetModel configSetModel)
        {
            var validationResults = source.Select(kvp => confgiurationValidator.Validate(kvp.Value, configSetModel.Configs.Single(s => s.Name == kvp.Key)));
            return new ValidationResult(validationResults);
        }

        private ConfigurationSetModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return configCollection.First(s => s.Configs.Any(a => a.Type == configInstance.ConfigType));
        }
    }
}
