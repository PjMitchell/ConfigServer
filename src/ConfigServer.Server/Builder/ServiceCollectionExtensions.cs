using ConfigServer.Core;
using ConfigServer.Server.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Server
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigServerServices(this IServiceCollection collection)
        {
            collection.AddMemoryCache();
            collection.Add(ServiceDescriptor.Transient<IHttpResponseFactory, HttpResponseFactory>());
            collection.Add(ServiceDescriptor.Transient<IResourceStore, EmptyResourceStore>());

            collection.Add(ServiceDescriptor.Transient<IConfigurationSetModelPayloadMapper, ConfigurationSetModelPayloadMapper>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationEditModelMapper, ConfigurationEditModelMapper>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationUpdatePayloadMapper, ConfigurationUpdatePayloadMapper>());

            collection.Add(ServiceDescriptor.Transient<IPropertyTypeProvider, PropertyTypeProvider>());

            collection.Add(ServiceDescriptor.Transient<IConfigInstanceRouter, ConfigInstanceRouter>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationSetService, ConfigurationSetService>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationClientService, ConfigurationClientService>());
            collection.Add(ServiceDescriptor.Transient<IOptionSetFactory, OptionSetFactory>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationSetFactory, ConfigurationSetFactory>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationValidator, ConfigurationValidator>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationSetUploadMapper, ConfigurationSetUploadMapper>());
            collection.Add(ServiceDescriptor.Transient<IConfigurationService, ConfigurationService>());
            collection.AddConfigServerEndPoints()
                .AddConfigServerCommandHandlers()
                .AddConfigServerEventHandlers();


            return collection;
        }

        private static IServiceCollection AddConfigServerEndPoints(this IServiceCollection collection)
        {
            collection.Add(ServiceDescriptor.Transient<ConfigurationSetEnpoint, ConfigurationSetEnpoint>());
            collection.Add(ServiceDescriptor.Transient<ConfigClientEndPoint, ConfigClientEndPoint>());
            collection.Add(ServiceDescriptor.Transient<ConfigManagerEndpoint, ConfigManagerEndpoint>());
            collection.Add(ServiceDescriptor.Transient<ConfigEnpoint, ConfigEnpoint>());
            collection.Add(ServiceDescriptor.Transient<DownloadEndpoint, DownloadEndpoint>());
            collection.Add(ServiceDescriptor.Transient<UploadEnpoint, UploadEnpoint>());
            collection.Add(ServiceDescriptor.Transient<ResourceEndpoint, ResourceEndpoint>());
            collection.Add(ServiceDescriptor.Transient<ClientGroupEndpoint, ClientGroupEndpoint>());
            return collection;
        }

        private static IServiceCollection AddConfigServerCommandHandlers(this IServiceCollection collection)
        {
            collection.Add(ServiceDescriptor.Transient<ICommandBus, CommandBus>());
            collection.Add(ServiceDescriptor.Transient<ICommandHandler<CreateUpdateClientGroupCommand>, CreateUpdateClientGroupCommandHandler>());
            collection.Add(ServiceDescriptor.Transient<ICommandHandler<CreateUpdateClientCommand>, CreateUpdateClientCommandHandler>());
            collection.Add(ServiceDescriptor.Transient<ICommandHandler<UpdateConfigurationFromEditorCommand>, UpdateConfigurationFromEditorCommandHandler>());

            return collection;
        }

        private static IServiceCollection AddConfigServerEventHandlers(this IServiceCollection collection)
        {
            collection.Add(ServiceDescriptor.Transient<IEventService, EventService>());
            collection.Add(ServiceDescriptor.Transient<IEventHandler<ConfigurationUpdatedEvent>, ConfigurationUpdatedEventHandler>());
            collection.Add(ServiceDescriptor.Transient<IEventHandler<ConfigurationClientGroupUpdatedEvent>, ConfigurationClientGroupUpdatedEventHandler>());
            collection.Add(ServiceDescriptor.Transient<IEventHandler<ConfigurationClientUpdatedEvent>, ConfigurationClientUpdatedEventHandler>());
            return collection;
        }
    }
}
