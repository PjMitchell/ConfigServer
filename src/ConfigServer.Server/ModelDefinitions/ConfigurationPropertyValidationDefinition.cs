﻿using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Validation information for a configuration property
    /// </summary>
    public class ConfigurationPropertyValidationDefinition
    {
        /// <summary>
        /// Minimum value for a property
        /// </summary>
        public IComparable Min { get; set; }

        /// <summary>
        /// Maximum value for a property
        /// </summary>
        public IComparable Max { get; set; }

        /// <summary>
        /// Maximum length for a property
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Regrex pattern a string property must match
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Is property Required
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
