namespace ConfigServer.Server
{
    /// <summary>
    /// Option object for use in input selection
    /// </summary>
    public class OptionSelection
    {
        /// <summary>
        /// Initializes Option selection
        /// </summary>
        public OptionSelection()
        {

        }

        /// <summary>
        /// Initializes Option selection with values
        /// </summary>
        /// <param name="key">Key value</param>
        /// <param name="displayValue">Display value</param>
        public OptionSelection(string key, string displayValue)
        {
            Key = key;
            DisplayValue = displayValue;
        }

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
