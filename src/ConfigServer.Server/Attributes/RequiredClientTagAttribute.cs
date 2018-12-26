using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Sets Required Tag for ConfigurationSet
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredClientTagAttribute : Attribute
    {
        /// <summary>
        /// Constructor for RequiredClientTagAttribute
        /// </summary>
        /// <param name="tagValue">Required Client Tag</param>
        public RequiredClientTagAttribute(string tagValue)
        {
            TagValue = tagValue;
        }

        /// <summary>
        /// Required Client Tag
        /// </summary>
        public string TagValue { get; }
    }
}
