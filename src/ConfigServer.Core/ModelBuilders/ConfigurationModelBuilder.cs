using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class ConfigurationModelBuilder<TConfig>
    {
        private readonly ConfigurationModelDefinition definition;

        public ConfigurationModelBuilder(ConfigurationModelDefinition definition)
        {
            this.definition = definition;
        }

        public ConfigurationModelDefinition Build() => definition;

        public ConfigurationPropertyBuilder Property<TProperty>(Expression<Func<TConfig,TProperty>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            ConfigurationPropertyDefinition value;
            if (!definition.ConfigurationProperties.TryGetValue(body.Member.Name, out value))
            {
                value = new ConfigurationPropertyDefinition(body.Member.Name);
                definition.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }

            return new ConfigurationPropertyBuilder(value);
        }
    }
}
