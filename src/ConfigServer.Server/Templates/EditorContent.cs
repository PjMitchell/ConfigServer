using ConfigServer.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigServer.Server.Templates
{
    internal class EditorContent
    {
        private IServiceProvider serviceProvider;

        public EditorContent(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string GetContent(ConfigurationClient client, ConfigInstance config, ConfigurationModel modelDefinition)
        {
            var configItem = config.GetConfiguration();
            var editFields = modelDefinition.GetPropertyDefinitions()
                .Select(prop => GetEditField(prop.GetPropertyValue(configItem),prop));
            return $@"
            <h3>Edit {client.Name} - {modelDefinition.ConfigurationDisplayName}</h3>
            <p>{modelDefinition.ConfigurationDescription}</p>
            <form method=""post"">
            { string.Join<string>("<br>", editFields)}
            <input type=""submit"" value=""Submit"">
            </form>";
        }

        private string GetEditField(object value,ConfigurationPropertyModelBase definition)
        {
            var description = string.IsNullOrWhiteSpace(definition.PropertyDescription) 
                ? string.Empty 
                : $"<br>{definition.PropertyDescription}";
            return  $"{definition.PropertyDisplayName}:{description}<br>{GetInputElement(value, (dynamic)definition)}<br>";
        }


        private string GetInputElement(object value, ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            var options = definition.GetAvailableOptions(serviceProvider)
                .Select(s=> $"<option value=\"{s.Key}\" {GetOptionSelectedTag(definition,s, value)}>{s.DisplayValue}</option>");
            return $@"
            <select name=""{definition.ConfigurationPropertyName}"">
                {string.Join(Environment.NewLine, options)}
            </select>";
        }

        private string GetOptionSelectedTag(ConfigurationPropertyWithOptionsModelDefinition definition,ConfigurationPropertyOptionDefintion optionDef, object value)
        {
            return definition.OptionMatchesKey(optionDef.Key, value) ? "selected" : string.Empty;
        }

        private string GetInputElement(object value, ConfigurationPrimitivePropertyModel definition)
        {
            if(IsIntergerType(definition.PropertyType))
                return IntergerInputTemplate.Build(value, definition);
            if(IsFloatType(definition.PropertyType))
                return FloatInputTemplate.Build(value, definition);
            if (definition.PropertyType == typeof(string))
                return StringInputTemplate.Build(value, definition);
            if (definition.PropertyType == typeof(bool))
                return GetInputElementForBool(value, definition.ConfigurationPropertyName);
            if (definition.PropertyType == typeof(DateTime))
                return DateTimeInputTemplate.Build(value, definition);
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                return GetInputElementForEnum(value, definition.ConfigurationPropertyName);
            return "Could not create Editor for property";
        }

        private static string GetInputElementForBool(object value, string name)
        {
            var b = (bool)value;
            const string check = "checked";
            var t = b ? check : string.Empty;
            var f = !b ? check : string.Empty;
            return $"<input type=\"radio\" name=\"{name}\" value=\"true\" {t}>True<br><input type=\"radio\" name=\"{name}\" value=\"false\" {f}> False";
        }

        private static string GetInputElementForEnum(object value, string name)
        {
            var enumValue = (Enum)value;
            const string check = "checked";
            //var t = b ? check : string.Empty;
            //var f = !b ? check : string.Empty;
            var stringBuilder = new StringBuilder();
            foreach(var option in Enum.GetNames(value.GetType()))
            {
                var isChecked = option == enumValue.ToString() ? check :string.Empty;
                stringBuilder.Append($"<input type=\"radio\" name=\"{name}\" value=\"{option}\" {isChecked}> {option}");
            }
            return stringBuilder.ToString();
        }

        private static bool IsIntergerType(Type type)
        {
            return type == typeof(int)
                || type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong);
        }

        private static bool IsFloatType(Type type)
        {
            return type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal);
        }
    }
}
