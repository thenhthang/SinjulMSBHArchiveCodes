using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MultipleLoginPages.Data;

namespace MultipleLoginPages
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //await Initial(host);

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
                    UserName = "Customer@Mail.com",
                    Email = "Customer@Mail.com",
                    EmailConfirmed = true,
                };

                await usermanager.CreateAsync(admin, "Pa$$w0rd");
                await usermanager.CreateAsync(user, "Pa$$w0rd");

                await rolemanager.CreateAsync(new IdentityRole("Admin_Role"));
                await rolemanager.CreateAsync(new IdentityRole("Customer_Role"));

                await usermanager.AddToRoleAsync(admin, "Admin_Role");
                await usermanager.AddToRoleAsync(user, "Customer_Role");
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
