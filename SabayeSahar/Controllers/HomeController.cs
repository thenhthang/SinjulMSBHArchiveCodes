using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SabayeSahar.Data;
using SabayeSahar.Models;

namespace SabayeSahar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async ValueTask<OkObjectResult> IndexAsync(
            [FromServices] ApplicationDbContext context,
            CancellationToken cancellationToken = default)
        {
            Student student = new Student("Sinjul", "MSBH", "09215892274");

            Order order = new Order(student, 13, 1300);

            await context.Orders.AddAsync(order, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            //◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘◘

            Order order2 = new Order(student.Id, 13000, "13", true);

            await context.Orders.AddAsync(order2, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new { student, order, order2 });
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
