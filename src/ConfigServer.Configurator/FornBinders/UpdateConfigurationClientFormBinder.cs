using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ConfigServer.Configurator
{
    internal static class UpdateConfigurationClientFormBinder
    {
        public static ConfigurationClient BindForm(IFormCollection collection)
        {
            var result = new ConfigurationClient
            {
                ClientId = collection["ClientId"].Single(),
                Name = collection["Name"].Single(),
                Description = collection["Description"].Single()
            };
            return result;
        }
    }
}
