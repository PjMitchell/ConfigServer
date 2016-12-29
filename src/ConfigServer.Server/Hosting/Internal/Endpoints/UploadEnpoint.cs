using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;

namespace ConfigServer.Server
{
    internal class UploadEnpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigurationValidator confgiurationValidator;

        public UploadEnpoint(IConfigHttpResponseFactory responseFactory, IConfigInstanceRouter configInstanceRouter, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator confgiurationValidator)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.configInstanceRouter = configInstanceRouter;
            this.configRepository = configRepository;
            this.confgiurationValidator = confgiurationValidator;
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

        private ConfigurationSetModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return configCollection.First(s => s.Configs.Any(a => a.Type == configInstance.ConfigType));
        }
    }
}
