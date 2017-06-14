using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigServer.Gui.Models;
using System.Security.Claims;
using ConfigServer.Server;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ConfigServer.Gui.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserProvider userProvider;

        public HomeController(UserProvider userProvider)
        {
            this.userProvider = userProvider;
        }
        
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserSetup()
        {
            var principal = userProvider.GetPrincipal();
            var claimModel = new UserClaimModel();
            claimModel.Username = principal.Identity.Name;
            claimModel.ClientAdmin = principal.FindFirst(c => c.Type == ConfigServerConstants.ClientAdminClaimType)?.Value ?? string.Empty;
            claimModel.ReadClaims = string.Join(",", principal.FindAll(ConfigServerConstants.ClientReadClaimType).Select(s => s.Value));
            claimModel.ConfiguratorClaim = string.Join(",", principal.FindAll(ConfigServerConstants.ClientConfiguratorClaimType).Select(s => s.Value));

            return View(claimModel);
        }

        [HttpPost]
        public IActionResult UserSetup(UserClaimModel claimModel)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(UserProvider.username, claimModel.Username));
            if (!string.IsNullOrWhiteSpace(claimModel.ClientAdmin))
                claims.Add(new Claim(ConfigServerConstants.ClientAdminClaimType, claimModel.ClientAdmin));
            if (!string.IsNullOrWhiteSpace(claimModel.ReadClaims))
                claims.AddRange(claimModel.ReadClaims.Split(',').Select(s => new Claim(ConfigServerConstants.ClientReadClaimType, s)));
            if (!string.IsNullOrWhiteSpace(claimModel.ConfiguratorClaim))
                claims.AddRange(claimModel.ConfiguratorClaim.Split(',').Select(s => new Claim(ConfigServerConstants.ClientConfiguratorClaimType, s)));
            userProvider.SetPrincipal(claims);
            return RedirectToAction(nameof(Index));
        }
    }
}
