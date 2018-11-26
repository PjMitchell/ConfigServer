namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer options
    /// </summary>
    public class ConfigServerOptions
    {
        /// <summary>
        /// Allows Anomynous access for config manager if no required claims have been specified
        /// </summary>
        public bool AllowManagerAnomynousAccess { get; set; }

        /// <summary>
        /// Allows Anomynous access for config if no required claims have been specified
        /// </summary>
        public bool AllowAnomynousAccess { get; set; }

        /// <summary>
        /// Claim type for access to configurator screen Admin users can add  clients and client groups.
        /// default configserver_clientadmin
        /// empty ClaimType will give all authenticated users the ability to create clients and client groups
        /// Claim value can be configurator/admin
        /// </summary>
        public string ClientAdminClaimType { get; set; } = ConfigServerConstants.ClientAdminClaimType;

        /// <summary>
        /// Claim type for read access to config.
        /// default configserver_clientread
        /// empty ClaimType will give all authenticated users the ability to read all clients
        /// Required Claim value defined by each client 
        /// </summary>
        public string ClientReadClaimType { get; set; } = ConfigServerConstants.ClientReadClaimType;

        /// <summary>
        /// Claim type for configurator access to config.
        /// default configserver_clientconfigurator
        /// empty ClaimType will give all authenticated users the ability to configurate all clients
        /// Required Claim value defined by each client 
        /// </summary>
        public string ClientConfiguratorClaimType { get; set; } = ConfigServerConstants.ClientConfiguratorClaimType;

        /// <summary>
        /// Url to Themes css for the config management screen. See Angular Material themes
        /// </summary>
        public string ThemeUrl { get; set; }
    }
}
