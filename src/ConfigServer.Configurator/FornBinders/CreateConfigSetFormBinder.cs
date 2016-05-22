using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ConfigServer.Configurator
{
    public static class CreateConfigSetFormBinder
    {
        public static string BindForm(IFormCollection collection)
        {
           return  collection["ConfigSetId"].Single();
        }
    }
}
