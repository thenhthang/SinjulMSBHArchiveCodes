using System.Diagnostics;

using JsonLogger.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JsonLogger.Controllers
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
            _logger.LogInformation("'LogInformation' Hello from SinjulMSBH .. !!!!");
            _logger.LogWarning("'LogWarning' Hello from SinjulMSBH .. !!!!");
            _logger.LogError("'LogError' ello from SinjulMSBH .. !!!!");
            _logger.LogWarning("'LogWarning' Hello from SinjulMSBH .. !!!!");

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
