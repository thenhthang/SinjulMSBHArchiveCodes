
using Microsoft.AspNetCore.Mvc;

namespace AuthorizeArea.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index() => View();
    }
}
