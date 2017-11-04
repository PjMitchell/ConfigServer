using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConfigServer.Core;
using ConfigServer.Sample.mvcApp2.Models;

namespace ConfigServer.Sample.mvcApp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigServer configProvider;

        public HomeController(IConfigServer configProvider)
        {
            this.configProvider = configProvider;
        }
        
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var config = await configProvider.GetConfigAsync<SampleConfig>();
            var options = await configProvider.GetCollectionConfigAsync<OptionFromConfigSet>();

            return View(new ConfigViewModel { Config = config, Options = options });
        }
    }

    public class ConfigViewModel
    {
        public SampleConfig Config { get; set; }
        public IEnumerable<OptionFromConfigSet> Options { get; set; }
    }
}
