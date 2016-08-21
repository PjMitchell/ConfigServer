using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Model that has properties, Used in model builder
    /// </summary>
    /// <typeparam name="TModel">Type Model is representing</typeparam>
    public interface IModelWithProperties<TModel>
    {
        /// <summary>
        /// Property definitions for model
        /// </summary>
        Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties { get; }
    }
}
