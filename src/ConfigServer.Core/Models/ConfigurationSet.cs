﻿using System.Linq;
using System.Reflection;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a set of configurations and sets up the information required to build, configure and validate the configurations.
    /// </summary>
    public abstract class ConfigurationSet
    {
        /// <summary>
        /// Display name for configuartion set
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Description for configuartion set
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Intializes configuration set
        /// </summary>
        protected ConfigurationSet()
        {

        }

        /// <summary>
        /// Intializes configuration set with name
        /// </summary>
        /// <param name="name">Display name for configuartion set</param>
        protected ConfigurationSet(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Intializes configuration set with name and description
        /// </summary>
        /// <param name="name">Display name for configuartion set</param>
        /// <param name="description">Description for configuartion set</param>
        protected ConfigurationSet(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Builds the ConfigurationSetModel that contains the information required to build, configure and validate the configuration
        /// </summary>
        /// <returns>Initialized ConfigurationSetModel</returns>
        public ConfigurationSetModel BuildConfigurationSetModel()
        {
            var builder =new ConfigurationSetModelBuilder(this.GetType(), Name?? this.GetType().Name, Description);
            OnModelCreation(builder);
            return builder.Build();
        }

        /// <summary>
        /// overridden to declare how the configuration set sets up the information required to build, configure and validate the configurations.
        /// </summary>
        /// <param name="modelBuilder">ConfigurationSetModelBuilder used to construct ConfigurationSetModel</param>
        protected virtual void OnModelCreation(ConfigurationSetModelBuilder modelBuilder)
        {
            foreach (var propertyInfo in this.GetType().GetProperties().Where(info =>info.PropertyType.IsConstructedGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Config<>)))
            {
                modelBuilder.AddConfig(propertyInfo.PropertyType.GenericTypeArguments[0]);
            }
        }
    }
}
