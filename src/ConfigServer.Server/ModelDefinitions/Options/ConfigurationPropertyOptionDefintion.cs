namespace ConfigServer.Server
{
    /// <summary>
    /// Option for a property
    /// </summary>
    public class ConfigurationPropertyOptionDefintion
    {
        /// <summary>
        /// Key used to identity option and match it source
        /// </summary>
        public string Key {get; set;}
        /// <summary>
        /// Display value for option
        /// </summary>
        public string DisplayValue {get; set;}
    }
}
