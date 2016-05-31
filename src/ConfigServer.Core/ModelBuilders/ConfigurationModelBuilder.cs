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

        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, byte>> expression) => CreateForInterger(expression);
        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, sbyte>> expression) => CreateForInterger(expression);
        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, int>> expression) => CreateForInterger(expression);
        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, uint>> expression) => CreateForInterger(expression);
        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, long>> expression) => CreateForInterger(expression);
        public ConfigurationIntergerPropertyBuilder Property(Expression<Func<TConfig, ulong>> expression) => CreateForInterger(expression);

        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, float>> expression) => CreateForFloat(expression);
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, double>> expression) => CreateForFloat(expression);
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, decimal>> expression) => CreateForFloat(expression);

        public ConfigurationBoolPropertyBuilder Property(Expression<Func<TConfig, bool>> expression) => new ConfigurationBoolPropertyBuilder(GetOrAddPropertyDefinition(expression));

        public ConfigurationStringPropertyBuilder Property(Expression<Func<TConfig, string>> expression) => new ConfigurationStringPropertyBuilder(GetOrAddPropertyDefinition(expression));

        public ConfigurationDateTimePropertyBuilder Property(Expression<Func<TConfig, DateTime>> expression) => new ConfigurationDateTimePropertyBuilder(GetOrAddPropertyDefinition(expression));

        public ConfigurationEnumPropertyBuilder Property(Expression<Func<TConfig, Enum>> expression) => new ConfigurationEnumPropertyBuilder(GetOrAddPropertyDefinition(expression));


        private ConfigurationIntergerPropertyBuilder CreateForInterger(LambdaExpression expression)
        {
            return new ConfigurationIntergerPropertyBuilder(GetOrAddPropertyDefinition(expression));
        }

        private ConfigurationFloatPropertyBuilder CreateForFloat(LambdaExpression expression)
        {
            return new ConfigurationFloatPropertyBuilder(GetOrAddPropertyDefinition(expression));
        }

        private ConfigurationPropertyDefinition GetOrAddPropertyDefinition(LambdaExpression expression)
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

            return value;
        }
    }
}
