using System.Threading.Tasks;

using AuthorizeArea.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthorizeArea
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // await Initial(host);

            await host.RunAsync();

            static async Task Initial(IHost host)
            {
                using var scope = host.Services.CreateScope();

                await scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>()
                        .Database.EnsureCreatedAsync()
                ;

                var usermanager =
                    scope.ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();

                var rolemanager =
                    scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                var admin = new IdentityUser
                {
                    UserName = "Admin@Mail.com",
                    Email = "Admin@Mail.com",
                    EmailConfirmed = true,
                };

                var user = new IdentityUser
                {
                    UserName = "User@Mail.com",
                    Email = "User@Mail.com",
                    EmailConfirmed = true,
                };

                await usermanager.CreateAsync(admin, "Pa$$w0rd");
                await usermanager.CreateAsync(user, "Pa$$w0rd");

                await rolemanager.CreateAsync(new IdentityRole("AdminRole"));
                await rolemanager.CreateAsync(new IdentityRole("UserRole"));

                await usermanager.AddToRoleAsync(admin, "AdminRole");
                await usermanager.AddToRoleAsync(user, "UserRole");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
