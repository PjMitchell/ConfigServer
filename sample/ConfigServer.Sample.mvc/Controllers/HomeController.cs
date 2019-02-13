using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigServer.Sample.mvc.Models;
using ConfigServer.Core;

namespace ConfigServer.Sample.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigServer configServer;

        public HomeController(IConfigServer configServer)
        {
            this.configServer = configServer;
        }

        public async Task<IActionResult> Index()
        {
            var config = await configServer.GetConfigAsync<SampleConfig>();
            var options = await configServer.GetCollectionConfigAsync<OptionFromConfigSet>();
            return View(new ConfigViewModel { Config = config, Options = options});
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        
    }

    public class ConfigViewModel
    {
        public SampleConfig Config { get; set; }
        public IEnumerable<OptionFromConfigSet> Options { get; set; }

    }
}
