using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConfigServer.Gui.Models
{
    public class UserProvider
    {
        private ClaimsPrincipal principal;
        public const string username = "name";
        public const string role = "role";
        public const string auth = "hkd";

        public UserProvider()
        {
            var claims = new List<Claim>
            {
                new Claim(username, "A.Person"),
                new Claim(Constants.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue)
            };
            SetPrincipal(claims);
        }

        public ClaimsPrincipal GetPrincipal() => principal;

        public void SetPrincipal(IEnumerable<Claim> claims) 
        {
            var identity = new ClaimsIdentity(claims, auth, username, role);
            principal = new ClaimsPrincipal(identity);
        }
    }
}
