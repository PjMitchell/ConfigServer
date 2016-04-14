using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ConfigServer.Sample.mvc.Models;

namespace ConfigServer.Sample.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly SampleConfig config;

        public HomeController(SampleConfig config)
        {
            this.config = config;
        }

        public IActionResult Index()
        {
            return View(config);
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
}
