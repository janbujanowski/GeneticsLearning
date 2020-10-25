using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neurox.ConsoleGUI;
using Neurox.Web.Models;

namespace Neurox.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            GeneticEnvironment genParams = GeneticEnvironment.INSTANCE;
            genParams.ParseParameters("\"20/12/2019 23:39:30\" 10 0.15 5 Tournament Basic 1000 1  0 60");
            NeuroxEvolution evolution = new NeuroxEvolution(genParams);
            DashboardViewModel viewModel = new DashboardViewModel();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
