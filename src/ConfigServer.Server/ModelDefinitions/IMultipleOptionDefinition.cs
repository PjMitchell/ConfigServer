using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    interface IPropertyDefinition
    {
        object GetPropertyValue(object config);
        void SetPropertyValue(object config, object value);
        string ConfigurationPropertyName { get; }
        string PropertyDisplayName { get; }
        string PropertyDescription { get; }
        Type PropertyType { get; }
        IEnumerable<ConfigurationDependency> GetDependencies();
    }


    interface IOptionPropertyDefinition : IPropertyDefinition
    {
    }

    interface IMultipleOptionPropertyDefinition : IOptionPropertyDefinition
    {
        CollectionBuilder GetCollectionBuilder();
    }
}
