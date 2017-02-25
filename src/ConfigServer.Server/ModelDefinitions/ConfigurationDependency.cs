using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Depenedency for Configuration
    /// </summary>
    public class ConfigurationDependency 
    {
        /// <summary>
        /// Constructs new instance of ConfigurationDependency
        /// </summary>
        /// <param name="configurationSet">Type of Configuration set that the dependency is drawn from</param>
        /// <param name="propertyPath">The Path from the ConfigurationSet To the dependency</param>
        public ConfigurationDependency(Type configurationSet, string propertyPath)
        {
            ConfigurationSet = configurationSet;
            PropertyPath = propertyPath;
        }

        /// <summary>
        /// The Path from the ConfigurationSet To the dependency
        /// </summary>
        public string PropertyPath { get; }

        /// <summary>
        /// The Path from the ConfigurationSet To the dependency
        /// </summary>
        public Type ConfigurationSet { get; }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="obj">The string to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            var dependency = obj as ConfigurationDependency;
            return dependency != null
                && ConfigurationSet != dependency.ConfigurationSet
                && PropertyPath != dependency.PropertyPath;

        }

        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (397 * (ConfigurationSet?.GetHashCode() ?? 0)) ^ ((PropertyPath?.GetHashCode() ?? 0));
                return hashCode;
            }
        }
    }
}
