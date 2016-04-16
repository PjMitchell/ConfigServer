using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Configurator.Templates
{
    public static class CreateConfigSetContent
    {
        public static string GetContent()
        {
            var guid = Guid.NewGuid();
            return $@"
            <h3>Create Config Set</h3>
            <form method=""post"">
            <input type =""text"" name=""ConfigSetId"" value=""{guid}"">
            <input type=""submit"" value=""Submit"">
            </form>";
        }
    }
}
