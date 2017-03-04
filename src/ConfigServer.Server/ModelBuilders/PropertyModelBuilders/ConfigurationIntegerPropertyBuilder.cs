﻿using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Integer values such as bytes, ints and longs 
    /// </summary>
    public class ConfigurationIntegerPropertyBuilder<TPropertyType> : ConfigurationPropertyModelBuilder<ConfigurationIntegerPropertyBuilder<TPropertyType>> where TPropertyType : IComparable
    {
        /// <summary>
        /// Initializes ConfigurationIntegerPropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        internal ConfigurationIntegerPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }

        /// <summary>
        /// Sets Maximum value validation rule for property
        /// </summary>
        /// <param name="value">Maximum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationIntegerPropertyBuilder<TPropertyType> WithMaxValue(TPropertyType value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Max = value;
            return this;
        }

        /// <summary>
        /// Sets Minimum value validation rule for property
        /// </summary>
        /// <param name="value">Minimum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationIntegerPropertyBuilder<TPropertyType> WithMinValue(TPropertyType value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Min = value;
            return this;
        }
    }

}
