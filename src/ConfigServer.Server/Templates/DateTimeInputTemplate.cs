using ConfigServer.Core;
using System;
using System.Text;

namespace ConfigServer.Server.Templates
{
    internal static class DateTimeInputTemplate
    {
        public static string Build(object value, ConfigurationPrimitivePropertyModel definition, string namePrefix = "")
        {
            return $"<input type=\"datetime\" name=\"{namePrefix}{definition.ConfigurationPropertyName}\" value=\"{value}\" {BuildValidationElement(definition)}>";
        }

        private static string BuildValidationElement(ConfigurationPrimitivePropertyModel definition)
        {
            var builder = new StringBuilder();
            if (definition.ValidationRules.Min != null)
                builder.Append($"min=\"{GetDateString(definition.ValidationRules.Min)}\" ");
            if (definition.ValidationRules.Max != null)
                builder.Append($"max=\"{GetDateString(definition.ValidationRules.Max)}\" ");
            return builder.ToString();
        }

        private static string GetDateString(object value)
        {
            return ((DateTime)value).ToString("yyyy-MM-dd");
        }
    }
}
