using System;
using System.Security.Claims;

namespace ConfigServer.Server
{
    /// <summary>
    /// Authentication options for ConfigServer
    /// </summary>
    public class ConfigServerAuthenticationOptions
    {
        /// <summary>
        /// Flags if ConfigServer requires Authentication
        /// Default: true
        /// </summary>
        public bool RequireAuthentication { get; set; } = true;

        /// <summary>
        /// Role required to use ConfigServer
        /// </summary>
        public string RequiredRole { get; set; }

        /// <summary>
        /// Claim required to use ConfigServer
        /// </summary>
        public Predicate<Claim> RequiredClaim { get; set; }
    }
}
