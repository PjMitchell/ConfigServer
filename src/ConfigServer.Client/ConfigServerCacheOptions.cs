using System;

namespace ConfigServer.Client
{
    /// <summary>
    /// Options for Config server Configuration caching
    /// </summary>
    public class ConfigServerCacheOptions
    {
        /// <summary>
        /// Flag if caching is to be turned off
        /// </summary>
        public bool IsDisabled { get; set;}

        /// <summary>
        /// Absolute Expiration of Configuration in cache
        /// Default 5 minutes
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Sliding Expiration of Configuration in cache
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Default configuration for Cache
        /// </summary>
        public static ConfigServerCacheOptions Default => new ConfigServerCacheOptions { AbsoluteExpiration = TimeSpan.FromMinutes(5) };
    }
}
