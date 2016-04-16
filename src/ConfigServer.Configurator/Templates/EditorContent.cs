using ConfigServer.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigServer.Configurator.Templates
{
    public static class EditorContent
    {
        public static string GetContent(Config config)
        {
            var configItem = config.GetConfiguration();
            var editFields = config.ConfigType.GetProperties()
                .Where(prop => prop.CanWrite)
                .Select(prop => GetEditField(prop.GetValue(configItem),prop.PropertyType, prop.Name));
            return $@"
            <h3>Edit {config.ConfigSetId} - {config.Name}</h3>
            <form method=""post"">
            { string.Join<string>("<br>", editFields)}
            <input type=""submit"" value=""Submit"">
            </form>";
        }

        private static string GetEditField(object value,Type type, string name)
        {
            return  $"{name}:<br>{GetInputElement(value, type, name)}<br>";
        }

        private static string GetInputElement(object value, Type type, string name)
        {
            if(IsIntergerType(type))
                return GetInputElementForInterger(value, name);
            if(IsFloatType(type))
                return GetInputElementForFloat(value, name);
            if (type == typeof(string))
                return GetInputElementForString(value, name);
            if (type == typeof(bool))
                return GetInputElementForBool(value, name);
            if (type == typeof(DateTime))
                return GetInputElementForDateTime(value, name);
            if (typeof(Enum).IsAssignableFrom(type))
                return GetInputElementForEnum(value, name);
            return "Could not create Editor for property";
        }

        private static string GetInputElementForInterger(object value, string name)
        {
            return $"<input type=\"number\" name=\"{name}\" value=\"{value}\">";
        }

        private static string GetInputElementForFloat(object value, string name)
        {
            return $"<input type=\"number\" name=\"{name}\" value=\"{value}\" step=\"0.00001\">";
        }

        private static string GetInputElementForString(object value, string name)
        {
            return $"<input type=\"text\" name=\"{name}\" value=\"{value}\">";
        }

        private static string GetInputElementForDateTime(object value, string name)
        {
            return $"<input type=\"datetime\" name=\"{name}\" value=\"{value}\">";
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
