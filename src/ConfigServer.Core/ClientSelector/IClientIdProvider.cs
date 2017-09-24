namespace ConfigServer.Core
{
    /// <summary>
    /// Service that provides the current client Id
    /// </summary>
    public interface IClientIdProvider
    {
        /// <summary>
        /// Gets current client Id 
        /// </summary>
        /// <returns>Current ClientId</returns>
        string GetCurrentClientId();
    }
}
