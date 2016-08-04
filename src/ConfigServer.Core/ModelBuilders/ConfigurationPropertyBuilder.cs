namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// </summary>
    /// <typeparam name="T">Implementation of ConfigurationPropertyModelBuilder</typeparam>
    public abstract class ConfigurationPropertyModelBuilder<T> where T :ConfigurationPropertyModelBuilder<T>
    {
        /// <summary>
        /// ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder
        /// </summary>
        protected readonly ConfigurationPropertyModelBase model;

        /// <summary>
        /// Initializes Builder for ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        protected ConfigurationPropertyModelBuilder(ConfigurationPropertyModelBase model)
        {
            this.model = model;
        }

        /// <summary>
        /// Updates display name of property 
        /// </summary>
        /// <param name="name">Display name of property</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public T WithDisplayName(string name)
        {
            model.PropertyDisplayName = name;
            return (T)this;
        }
        /// <summary>
        /// Updates description of property 
        /// </summary>
        /// <param name="description">Description of property</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public T WithDescription(string description)
        {
            model.PropertyDescription = description;
            return (T)this;
        }
    }
}
