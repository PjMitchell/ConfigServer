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
            collection.AddTransient<IHttpResponseFactory, HttpResponseFactory>();
            collection.AddTransient<IResourceStore, EmptyResourceStore>();
            collection.AddTransient<IResourceArchive, EmptyResourceArchive>();

            collection.AddTransient<IConfigurationSetModelPayloadMapper, ConfigurationSetModelPayloadMapper>();
            collection.AddTransient<IConfigurationEditModelMapper, ConfigurationEditModelMapper>();
            collection.AddTransient<IConfigurationUpdatePayloadMapper, ConfigurationUpdatePayloadMapper>();

            collection.AddTransient<IPropertyTypeProvider, PropertyTypeProvider>();

            collection.AddTransient<IConfigInstanceRouter, ConfigInstanceRouter>();
            collection.AddTransient<IConfigurationSetService, ConfigurationSetService>();
            collection.AddTransient<IConfigurationClientService, ConfigurationClientService>();
            collection.AddTransient<IOptionSetFactory, OptionSetFactory>();
            collection.AddTransient<IConfigurationSetFactory, ConfigurationSetFactory>();
            collection.AddTransient<IConfigurationValidator, ConfigurationValidator>();
            collection.AddTransient<IConfigurationSetUploadMapper, ConfigurationSetUploadMapper>();
            collection.AddTransient<IConfigurationService, ConfigurationService>();
            collection.AddConfigServerEndPoints()
                .AddConfigServerCommandHandlers()
                .AddConfigServerEventHandlers();


            return collection;
        }

        private static IServiceCollection AddConfigServerEndPoints(this IServiceCollection collection)
        {
            collection.AddTransient<ConfigurationSetEnpoint>()
                .AddTransient<ConfigClientEndPoint>()
                .AddTransient<ConfigManagerEndpoint>()
                .AddTransient<ConfigEnpoint>()
                .AddTransient<DownloadEndpoint>()
                .AddTransient<UploadEnpoint>()
                .AddTransient<ResourceEndpoint>()
                .AddTransient<ClientGroupEndpoint>()
                .AddTransient<GuidGeneratorEndpoint>()
                .AddTransient<ResourceArchiveEndpoint>()
                .AddTransient<ConfigArchiveEndPoint>()
                .AddTransient<PermissionEndpoint>();
            return collection;
        }

        private static IServiceCollection AddConfigServerCommandHandlers(this IServiceCollection collection)
        {
            collection.AddTransient<ICommandBus, CommandBus>();
            collection.AddCommandHandler<CreateUpdateClientGroupCommand, CreateUpdateClientGroupCommandHandler>();
            collection.AddCommandHandler<CreateUpdateClientCommand, CreateUpdateClientCommandHandler>();
            collection.AddCommandHandler<UpdateConfigurationFromEditorCommand, UpdateConfigurationFromEditorCommandHandler>();
            collection.AddCommandHandler<UpdateConfigurationFromJsonUploadCommand, UpdateConfigurationFromJsonUploadCommandHandler>();
            collection.AddCommandHandler<UpdateConfigurationSetFromJsonUploadCommand, UpdateConfigurationSetFromJsonUploadCommandHandler>();
            
            return collection;
        }

        private static IServiceCollection AddConfigServerEventHandlers(this IServiceCollection collection)
        {
            collection.AddTransient<IEventService, EventService>();
            collection.AddEventHandler<ConfigurationUpdatedEvent, ConfigurationUpdatedEventHandler>();
            collection.AddEventHandler<ConfigurationClientGroupUpdatedEvent, ConfigurationClientGroupUpdatedEventHandler>();
            collection.AddEventHandler<ConfigurationClientUpdatedEvent, ConfigurationClientUpdatedEventHandler>();
            return collection;
        }

        private static IServiceCollection AddCommandHandler<TCommand, TCommandHandler>(this IServiceCollection collection) where TCommand : ICommand where TCommandHandler : class, ICommandHandler<TCommand>
        {
            collection.AddTransient<ICommandHandler<TCommand>, TCommandHandler>();
            return collection;
        }

        private static IServiceCollection AddEventHandler<TEvent, TEventHandler>(this IServiceCollection collection) where TEvent : IEvent where TEventHandler : class, IEventHandler<TEvent>
        {
            collection.AddTransient<IEventHandler<TEvent>, TEventHandler>();
            return collection;
        }
    }
}
