using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    /// <summary>
    /// ConfigServer client builder.
    /// Used in the initial configuration of the ConfigServer client
    /// </summary>
    public class ConfigServerClientBuilder
    {
        private readonly IServiceCollectionWrapper serviceCollection;

        /// <summary>
        /// Initializes new ConfigServerClientBuilder
        /// </summary>
        /// <param name="serviceCollection">Application Service collection</param>
        public ConfigServerClientBuilder(IServiceCollectionWrapper serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            var configurationCollection = new ConfigurationRegistry();
            ConfigurationRegistry = configurationCollection;
            serviceCollection.AddSingleton<IConfigurationRegistry>(configurationCollection);
        }

        /// <summary>
        /// Initializes new ConfigServerClientBuilder
        /// </summary>
        /// <param name="serviceCollection">Application Service collection</param>
        public ConfigServerClientBuilder(IServiceCollection serviceCollection) : this(new ServiceCollectionWrapper(serviceCollection))
        {
        }

        /// <summary>
        /// Add Transient service to builder
        /// </summary>
        /// <typeparam name="TInterface">Interface to implement</typeparam>
        /// <typeparam name="TService">Implementation</typeparam>
        public void AddTransient<TInterface, TService>() where TInterface : class where TService : class, TInterface => serviceCollection.AddTransient<TInterface,TService>();

        /// <summary>
        /// Add singleton service to builder
        /// </summary>
        /// <typeparam name="TService">Service</typeparam>
        /// <param name="service">Service instance</param>
        public void AddSingleton<TService>(TService service) where TService : class => serviceCollection.AddSingleton(service);

        /// <summary>
        /// ConfigurationRegistry for builder that forms a registry of available configurations types 
        /// </summary>
        public ConfigurationRegistry ConfigurationRegistry { get; }
    }
}
