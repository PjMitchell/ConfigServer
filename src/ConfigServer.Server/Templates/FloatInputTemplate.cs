using ConfigServer.Core;
using System.Text;

namespace ConfigServer.Server.Templates
{
    internal static class FloatInputTemplate
    {
        public static string Build(object value, ConfigurationPrimitivePropertyModel definition)
        {
            return $"<input type=\"number\" name=\"{definition.ConfigurationPropertyName}\" value=\"{value}\" step=\"0.00001\" {BuildValidationElement(definition)}>";
        }

        private static string BuildValidationElement(ConfigurationPrimitivePropertyModel definition)
        {
            var builder = new StringBuilder();
            if (definition.ValidationRules.Min != null)
                builder.Append($"min=\"{definition.ValidationRules.Min}\" ");
            if (definition.ValidationRules.Max != null)
                builder.Append($"max=\"{definition.ValidationRules.Max}\" ");
            return builder.ToString();
        }
    }
}
