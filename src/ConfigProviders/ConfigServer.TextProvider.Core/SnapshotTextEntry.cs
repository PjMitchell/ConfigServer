namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Snapshot Text entry
    /// </summary>
    public class SnapshotTextEntry
    {
        /// <summary>
        /// Creates new snapshot text entry
        /// </summary>
        public SnapshotTextEntry()
        {

        }

        /// <summary>
        /// Creates new snapshot text entry
        /// </summary>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="configurationJson">Configuration Json</param>
        public SnapshotTextEntry(string configurationName, string configurationJson)
        {
            ConfigurationName = configurationName;
            ConfigurationJson = configurationJson;

        }
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
