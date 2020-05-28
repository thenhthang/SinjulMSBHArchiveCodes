
using Microsoft.AspNetCore.Mvc;

namespace AuthorizeArea.Areas.User.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {
        public IActionResult Index() => View();
    }
}
