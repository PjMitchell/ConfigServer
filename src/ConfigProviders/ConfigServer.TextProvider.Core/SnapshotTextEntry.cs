namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Snapshot Text entry
    /// </summary>
    public class SnapshotTextEntry
    {
        /// <summary>
        /// Name of config
        /// </summary>
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Configuration object as json
        /// </summary>
        public string ConfigurationJson { get; set; }
    }
}
