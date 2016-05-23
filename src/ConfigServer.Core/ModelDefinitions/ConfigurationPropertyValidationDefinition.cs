namespace ConfigServer.Core
{
    public class ConfigurationPropertyValidationDefinition
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
    }
}
