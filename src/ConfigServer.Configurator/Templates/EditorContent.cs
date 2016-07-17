using ConfigServer.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigServer.Configurator.Templates
{
    internal static class EditorContent
    {
        public static string GetContent(ConfigurationClient client, Config config, ConfigurationModel modelDefinition)
        {
            var configItem = config.GetConfiguration();
            var editFields = config.ConfigType.GetProperties()
                .Where(prop => prop.CanWrite)
                .Select(prop => GetEditField(prop.GetValue(configItem),prop.PropertyType,modelDefinition.GetPropertyDefinition(prop.Name)));
            return $@"
            <h3>Edit {client.Name} - {config.Name}</h3>
            <form method=""post"">
            { string.Join<string>("<br>", editFields)}
            <input type=""submit"" value=""Submit"">
            </form>";
        }

        private static string GetEditField(object value,Type type, ConfigurationPropertyModel definition)
        {
            var description = string.IsNullOrWhiteSpace(definition.PropertyDescription) 
                ? string.Empty 
                : $"<br>{definition.PropertyDescription}";
            return  $"{definition.PropertyDisplayName}:{description}<br>{GetInputElement(value, type, definition)}<br>";
        }

        private static string GetInputElement(object value, Type type, ConfigurationPropertyModel definition)
        {
            if(IsIntergerType(type))
                return IntergerInputTemplate.Build(value, definition);
            if(IsFloatType(type))
                return FloatInputTemplate.Build(value, definition);
            if (type == typeof(string))
                return StringInputTemplate.Build(value, definition);
            if (type == typeof(bool))
                return GetInputElementForBool(value, definition.ConfigurationPropertyName);
            if (type == typeof(DateTime))
                return DateTimeInputTemplate.Build(value, definition);
            if (typeof(Enum).IsAssignableFrom(type))
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
