using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class SnapshotEndpoint : IEndpoint
    {
        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationSnapshotRepository snapShotRepository;
        readonly ICommandBus commandBus;
        readonly IConfigurationClientService configurationClientService;
        public SnapshotEndpoint(IConfigurationSnapshotRepository snapShotRepository, ICommandBus commandBus, IConfigurationClientService configurationClientService, IHttpResponseFactory httpResponseFactory)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.snapShotRepository = snapShotRepository;
            this.commandBus = commandBus;
            this.configurationClientService = configurationClientService;
        }

        public Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // / POST Save snapshot
            // /{snapShotId}DELETE: Deletes snapShot
            // /Group/{clientGroupId} GET: Gets SnapshotIds for clientGroupId
            // /{snapShotId}/to/{clientId}

            if (!CheckMethodAndAuthentication(context, options))
                return Task.FromResult(true);
            var pathParams = context.ToPathParams();

            switch(pathParams.Length)
            {
                case 0:
                    return HandleSaveSnapshot(context, options);
                case 1:
                    return HandleDeleteSnapshot(context, pathParams[0], options);
                case 2:
                    return HandleGetSnapshotForGroup(context, pathParams, options);
                case 3:
                    return HandlePushSnapshotToClient(context, pathParams, options);
                default:
                    httpResponseFactory.BuildNotFoundStatusResponse(context);
                    return Task.FromResult(true);
            }            
        }

        private async Task HandleSaveSnapshot(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method != "POST")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }

            var command = await context.GetObjectFromJsonBodyAsync<CreateSnapshotCommand>();
            var commmandResult = await commandBus.SubmitAsync(command);
            await httpResponseFactory.BuildResponseFromCommandResult(context, commmandResult);
        }

        private async Task HandleDeleteSnapshot(HttpContext context, string snapshotId, ConfigServerOptions options)
        {
            if (context.Request.Method != "DELETE")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }

            var command = new DeleteSnapshotCommand { SnapshotId = snapshotId };
            var commmandResult = await commandBus.SubmitAsync(command);
            await httpResponseFactory.BuildResponseFromCommandResult(context, commmandResult);
        }

        private async Task HandleGetSnapshotForGroup(HttpContext context,string[] pathParams, ConfigServerOptions options)
        {
            if(context.Request.Method != "GET")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }

            if (pathParams[0].Equals("group", StringComparison.OrdinalIgnoreCase))
            {
                var info = await snapShotRepository.GetSnapshots();
                await httpResponseFactory.BuildJsonResponse(context, info.Where(w => string.Equals(w.GroupId, pathParams[1], StringComparison.OrdinalIgnoreCase)));
                return;
            }

            httpResponseFactory.BuildNotFoundStatusResponse(context);
        }
                
        private async Task HandlePushSnapshotToClient(HttpContext context, string[] pathParams, ConfigServerOptions options)
        {
            if (context.Request.Method != "POST")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }
            var client = await configurationClientService.GetClientOrDefault(pathParams[2]);

            if (!context.ChallengeClientConfigurator(options, client, httpResponseFactory))
                return;

            var request = await context.GetObjectFromJsonBodyAsync<PushSnapshotToClientRequest>();
            if (client != null && request != null && pathParams[1].Equals("to", StringComparison.OrdinalIgnoreCase))
            { 
                var command = new PushSnapshotToClientCommand { SnapshotId = pathParams[0], TargetClient = client, ConfigsToCopy =  request.ConfigsToCopy ?? new string[0]};
                var commmandResult = await commandBus.SubmitAsync(command);
                await httpResponseFactory.BuildResponseFromCommandResult(context, commmandResult);
                return;
            }

            httpResponseFactory.BuildNotFoundStatusResponse(context);
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET"|| context.Request.Method == "POST" || context.Request.Method == "DELETE")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
