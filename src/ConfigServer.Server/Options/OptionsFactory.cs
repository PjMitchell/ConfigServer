using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server.Options
{
    /// <summary>
    /// Builds OptionSet for Definition
    /// </summary>
    public interface IOptionSetFactory
    {
        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <returns>OptionSet </returns>
        IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition);
    }

    /// <summary>
    /// Builds OptionSet for Definition
    /// </summary>
    public class OptionSetFactory : IOptionSetFactory
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Instantiates a new OptionSetFactory
        /// </summary>
        /// <param name="serviceProvider">Service Provider that has services for option provider</param>
        public OptionSetFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <returns>OptionSet </returns>
        public IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            return definition.BuildOptionSet(serviceProvider);
        }
    }
}
