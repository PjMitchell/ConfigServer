using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigServer.Gui.Models;
using System.Security.Claims;

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
            claimModel.ClientAdmin = principal.FindFirst(c => c.Type == Constants.ClientAdminClaimType)?.Value ?? string.Empty;
            return View(claimModel);
        }

        [HttpPost]
        public IActionResult UserSetup(UserClaimModel claimModel)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(UserProvider.username, claimModel.Username));
            if (!string.IsNullOrWhiteSpace(claimModel.ClientAdmin))
                claims.Add(new Claim(Constants.ClientAdminClaimType, claimModel.ClientAdmin));
            userProvider.SetPrincipal(claims);
            return RedirectToAction(nameof(Index));
        }
    }
}
