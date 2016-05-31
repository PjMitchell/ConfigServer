namespace ConfigServer.Core
{

    public class ConfigurationPropertyDefinition
    {
        public ConfigurationPropertyDefinition(string propertyName)
        {
            ConfigurationPropertyName = propertyName;
            PropertyDisplayName = propertyName;
            ValidationRules = new ConfigurationPropertyValidationDefinition();
        }

        public string ConfigurationPropertyName { get; }
        public string PropertyDisplayName { get; set; }
        public string PropertyDescription { get; set; }
        public ConfigurationPropertyValidationDefinition ValidationRules { get; set; }
    }
}
