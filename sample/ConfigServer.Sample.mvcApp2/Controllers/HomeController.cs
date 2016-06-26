using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigServer.Core;
using ConfigServer.Sample.mvcApp2.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ConfigServer.Sample.mvcApp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigServerClient configProvider;

        public HomeController(IConfigServerClient configProvider)
        {
            this.configProvider = configProvider;
        }
        
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var config = await configProvider.BuildConfigAsync<SampleConfig>();
            return View(config);
        }
    }
}
