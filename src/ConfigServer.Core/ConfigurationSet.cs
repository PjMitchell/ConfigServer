using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace ConfigServer.Core
{
    public abstract class ConfigurationSet
    {
        public ConfigurationSetDefinition BuildModelDefinition()
        {
            var builder = ConfigurationSetBuilder.Create(this.GetType());
            OnModelCreation(builder);
            return builder.Build();
        }

        protected virtual void OnModelCreation(ConfigurationSetBuilder modelBuilder)
        {
            foreach (var propertyInfo in this.GetType().GetProperties().Where(info =>info.PropertyType.GetGenericTypeDefinition() == typeof(Config<>)))
            {
                modelBuilder.AddConfig(propertyInfo.PropertyType.GenericTypeArguments[0]);
            }
        }
    }
}
