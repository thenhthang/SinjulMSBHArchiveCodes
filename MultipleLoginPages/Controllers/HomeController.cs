using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MultipleLoginPages.Models;

namespace MultipleLoginPages.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }







        //[Authorize(policy: "Admin_Policy", AuthenticationSchemes = "Admin_Scheme")]
        [Authorize(Roles = "Admin_Role", AuthenticationSchemes = "Admin_Scheme")]
        public ContentResult AdminAccount() => Content(nameof(AdminAccount));

        //[Authorize(policy: "Customer_Policy", AuthenticationSchemes = "Client_Scheme")]
        [Authorize(Roles = "Customer_Role", AuthenticationSchemes = "Client_Scheme")]
        public ContentResult CustomerAccount() => Content(nameof(CustomerAccount));


        public ContentResult AdminAccountLogin() => Content(nameof(AdminAccountLogin));
        public ContentResult CustomerAccountLogin() => Content(nameof(CustomerAccountLogin));
    }
}
