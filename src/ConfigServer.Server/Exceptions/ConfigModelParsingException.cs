using System;

namespace ConfigServer.Server
{
    class ConfigModelParsingException : Exception
    {
        public ConfigModelParsingException(string message) : base(message)
        {
        }
    }
}
