using CompanyEmployees.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Web.Controllers
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
            _logger.LogTrace($"{nameof(Index)} Action in {nameof(HomeController)}");
            _logger.LogDebug($"{nameof(Index)} Action in {nameof(HomeController)}");
            _logger.LogInformation($"{nameof(Index)} Action in {nameof(HomeController)}");
            _logger.LogWarning($"{nameof(Index)} Action in {nameof(HomeController)}");
            _logger.LogError($"{nameof(Index)} Action in {nameof(HomeController)}");
            _logger.LogCritical($"{nameof(Index)} Action in {nameof(HomeController)}");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
