using ConfigServer.Core;
using System;
using System.Collections;
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
                .Select(prop => GetEditField(prop.GetPropertyValue(configItem), prop));
            return $@"
            <h3>Edit {client.Name} - {modelDefinition.ConfigurationDisplayName}</h3>
            <p>{modelDefinition.ConfigurationDescription}</p>
            <form method=""post"">
            { string.Join<string>("<br>", editFields)}
            <input type=""submit"" value=""Submit"">
            </form>
            <script>
            function deleteRow(t,r) {{
             var i = r.parentNode.parentNode.rowIndex; document.getElementById(t).deleteRow(i);
            }}
            function addRow(t) {{
                var table =document.getElementById(t),len = table.rows.length,newRow = table.rows[len-1].cloneNode(true);table.appendChild( newRow );
            }}
            </script>";
        }

        private string GetEditField(object value,ConfigurationPropertyModelBase definition)
        {
            if (definition is ConfigurationCollectionPropertyDefinition)
                return GetCollectionEditField(value, (ConfigurationCollectionPropertyDefinition)definition);

            var description = string.IsNullOrWhiteSpace(definition.PropertyDescription) 
                ? string.Empty 
                : $"<br>{definition.PropertyDescription}";
            return  $"{definition.PropertyDisplayName}:{description}<br>{GetInputElement(value, (dynamic)definition)}<br>";
        }

        private string GetCollectionEditField(object value, ConfigurationCollectionPropertyDefinition definition)
        {
            var description = string.IsNullOrWhiteSpace(definition.PropertyDescription)
                ? string.Empty
                : $"<br>{definition.PropertyDescription}";
            var header = $"{definition.PropertyDisplayName}:{description}";
            var tableHeader = $"<tr>{string.Join("",definition.ConfigurationProperties.Select(s => $"<th>{s.Value.PropertyDisplayName}</th>"))}<th><button type=\"button\" onclick=\"addRow('{definition.ConfigurationPropertyName}')\">Add</button></td></th></tr>";

            var items = value as IEnumerable;
            var tableBuilder = new StringBuilder();
            if (items != null)
            {
                int row = 0;
                foreach (var item in items)
                {
                    tableBuilder.AppendLine($"<tr>{string.Join("", definition.ConfigurationProperties.Select(s => $"<td>{GetInputElement(s.Value.GetPropertyValue(item), (dynamic)s.Value, $"{definition.ConfigurationPropertyName}.")}</td>"))}<td><button type=\"button\" onclick=\"deleteRow('{definition.ConfigurationPropertyName}',this)\">Delete</button></td></tr>");
                    row++;
                }
            }
            //$"{definition.ConfigurationPropertyName}[{row}]."
            return $@"<table id=""{definition.ConfigurationPropertyName}"">
                {header}
                {tableHeader}
                {tableBuilder.ToString()}
            </table>";
        }


        private string GetInputElement(object value, ConfigurationPropertyWithOptionsModelDefinition definition, string namePrefix = "")
        {
            var options = definition.GetAvailableOptions(serviceProvider)
                .Select(s=> $"<option value=\"{s.Key}\" {GetOptionSelectedTag(definition,s, value)}>{s.DisplayValue}</option>");
            var multipleText = definition.IsMultiSelector ? "multiple" : string.Empty;
            return $@"
            <select name=""{namePrefix}{definition.ConfigurationPropertyName}"" {multipleText}>
                {string.Join(Environment.NewLine, options)}
            </select>";
        }

        private string GetOptionSelectedTag(ConfigurationPropertyWithOptionsModelDefinition definition,ConfigurationPropertyOptionDefintion optionDef, object value)
        {
            return definition.OptionMatchesKey(optionDef.Key, value) ? "selected" : string.Empty;
        }

        private string GetInputElement(object value, ConfigurationPrimitivePropertyModel definition, string namePrefix = "")
        {
            if(IsIntergerType(definition.PropertyType))
                return IntergerInputTemplate.Build(value, definition, namePrefix);
            if(IsFloatType(definition.PropertyType))
                return FloatInputTemplate.Build(value, definition, namePrefix);
            if (definition.PropertyType == typeof(string))
                return StringInputTemplate.Build(value, definition, namePrefix);
            if (definition.PropertyType == typeof(bool))
                return GetInputElementForBool(value, definition.ConfigurationPropertyName, namePrefix);
            if (definition.PropertyType == typeof(DateTime))
                return DateTimeInputTemplate.Build(value, definition, namePrefix);
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                return GetInputElementForEnum(value, definition.ConfigurationPropertyName,namePrefix);
            return "Could not create Editor for property";
        }

        private static string GetInputElementForBool(object value, string name, string namePrefix = "")
        {
            var b = (bool)value;
            const string check = "checked";
            var t = b ? check : string.Empty;
            var f = !b ? check : string.Empty;
            return $"<input type=\"radio\" name=\"{namePrefix}{name}\" value=\"true\" {t}>True<br><input type=\"radio\" name=\"{name}\" value=\"false\" {f}> False";
        }

        private static string GetInputElementForEnum(object value, string name, string namePrefix = "")
        {
            var enumValue = (Enum)value;
            const string check = "checked";
            //var t = b ? check : string.Empty;
            //var f = !b ? check : string.Empty;
            var stringBuilder = new StringBuilder();
            foreach(var option in Enum.GetNames(value.GetType()))
            {
                var isChecked = option == enumValue.ToString() ? check :string.Empty;
                stringBuilder.Append($"<input type=\"radio\" name=\"{namePrefix}{name}\" value=\"{option}\" {isChecked}> {option}");
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
