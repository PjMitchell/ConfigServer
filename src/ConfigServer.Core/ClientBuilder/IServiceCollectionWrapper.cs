using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    /// <summary>
    /// Wrapper for registering service
    /// </summary>
    public interface IServiceCollectionWrapper
    {
        /// <summary>
        /// Add item to service collection as singleton
        /// </summary>
        /// <typeparam name="TService">Service</typeparam>
        /// <param name="service">Service instance</param>
        void AddSingleton<TService>(TService service) where TService : class;

        /// <summary>
        /// Add Transient service to service collection
        /// </summary>
        /// <typeparam name="TInterface">Interface to implement</typeparam>
        /// <typeparam name="TService">Implementation</typeparam>
        void AddTransient<TInterface, TService>() where TInterface : class where TService: class, TInterface;
    }

    internal class ServiceCollectionWrapper : IServiceCollectionWrapper
    {
        private readonly IServiceCollection serviceCollection;

        public ServiceCollectionWrapper(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public void AddSingleton<TService>(TService implementationInstance) where TService : class => serviceCollection.AddSingleton(implementationInstance);

        public void AddTransient<TInterface, TService>() where TInterface : class where TService : class, TInterface => serviceCollection.AddTransient<TInterface, TService>();

    }
}
