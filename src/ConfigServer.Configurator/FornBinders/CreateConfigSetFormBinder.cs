using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
