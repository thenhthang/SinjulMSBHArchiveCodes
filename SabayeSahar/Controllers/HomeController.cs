using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            Student student = new Student(firstName: "Sinjul", lastName: "MSBH", phoneNumber: "09215892274")
            {
                //Id = 19 //! 19 == duplicate 
                Id = 26 //! 26 == unique
            };

            Order order = new Order(student: student, payTypeId: 13, amountPayed: 1300);

            await context.Orders.AddAsync(order, cancellationToken);

            //? SqlException: Cannot insert explicit value for identity column in table 'Students' when IDENTITY_INSERT is set to OFF.
            //? await context.SaveChangesAsync(cancellationToken);

            await ConnectionExecuteAsync(context, cancellationToken);

            return Ok(new { student, order });

            static async Task ConnectionExecuteAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
            {
                await context.Database.OpenConnectionAsync(cancellationToken);
                try
                {
                    //? SqlException: Violation of PRIMARY KEY constraint 'PK_Students'. 
                    //? Cannot insert duplicate key in object 'dbo.Students'.
                    //? The duplicate key value is (19).
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Students ON", cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);

                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Students OFF", cancellationToken);
                }
                finally
                {
                    await context.Database.CloseConnectionAsync();
                }
            }
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
