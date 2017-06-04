namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer options
    /// </summary>
    public class ConfigServerOptions
    {
        /// <summary>
        /// Allows Anomynous access if no required claims have been specified
        /// </summary>
        public bool AllowAnomynousAccess { get; set; }

        /// <summary>
        /// Claim type for access to create clients and client groups.
        /// default configserver_clientadmin
        /// empty ClaimType will give all authenticated users the ability to create clients and client groups
        /// Claim value can be read/write
        /// </summary>
        public string ClientAdminClaimType { get; set; } = ConfigServerConstants.ClientAdminClaimType;
    }
}
