﻿
namespace ConfigServer.Server
{
    /// <summary>
    /// Default Constants for ConfigServer
    /// </summary>
    public class ConfigServerConstants
    {
        /// <summary>
        /// Default ConfigServer Client Admin Claim Type
        /// </summary>
        public const string ClientAdminClaimType = "configserver_clientadmin";

        /// <summary>
        /// Default ConfigServer Client Read Claim Type
        /// </summary>
        public const string ClientReadClaimType = "configserver_clientread";

        /// <summary>
        /// Default ConfigServer Client Write Claim Type
        /// </summary>
        public const string ClientWriteClaimType = "configserver_clientwrite";

        /// <summary>
        /// Default ConfigServer configurator claim value
        /// </summary>
        public const string ConfiguratorClaimValue = "configurator";
        /// <summary>
        /// Default ConfigServer admin claim value
        /// </summary>
        public const string AdminClaimValue = "admin";
    }
}
