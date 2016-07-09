using System.Linq;
using System.Reflection;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a set of configurations and sets up the information required to build, configure and validate the configurations.
    /// </summary>
    public abstract class ConfigurationSet
    {
        /// <summary>
        /// Builds the ConfigurationSetModel that contains the information required to build, configure and validate the configuration
        /// </summary>
        /// <returns>Initialized ConfigurationSetModel</returns>
        public ConfigurationSetModel BuildConfigurationSetModel()
        {
            var builder =new ConfigurationSetModelBuilder(this.GetType());
            OnModelCreation(builder);
            return builder.Build();
        }

        /// <summary>
        /// overridden to declare how the configuration set sets up the information required to build, configure and validate the configurations.
        /// </summary>
        /// <param name="modelBuilder">ConfigurationSetModelBuilder used to construct ConfigurationSetModel</param>
        protected virtual void OnModelCreation(ConfigurationSetModelBuilder modelBuilder)
        {
            foreach (var propertyInfo in this.GetType().GetProperties().Where(info =>info.PropertyType.GetGenericTypeDefinition() == typeof(Config<>)))
            {
                modelBuilder.AddConfig(propertyInfo.PropertyType.GenericTypeArguments[0]);
            }
        }
    }
}
