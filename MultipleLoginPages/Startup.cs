using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MultipleLoginPages.Data;

namespace MultipleLoginPages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthentication()
               .AddCookie("Admin_Scheme",
                   options =>
                   {
                       options.LoginPath = "/Home/AdminAccountLogin";
                       options.ExpireTimeSpan = TimeSpan.FromHours(1);
                       options.Cookie.Name = "Admin";

                   })
               .AddCookie("Client_Scheme",
                   options =>
                   {
                       options.LoginPath = "/Home/CustomerAccountLogin";
                       options.ExpireTimeSpan = TimeSpan.FromHours(1);
                       options.Cookie.Name = "Client";

                   })
            ;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin_Policy",
                    authBuilder =>
                    {
                        authBuilder.AddAuthenticationSchemes("Admin_Scheme");
                        authBuilder.RequireRole("Admin_Role");
                    });

                options.AddPolicy("Customer_Policy",
                    authBuilder =>
                    {
                        authBuilder.AddAuthenticationSchemes("Client_Scheme");
                        authBuilder.RequireRole("Customer_Role");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            //app.UseCookiePolicy(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = "Phone",
            //    LoginPath = "<phone - path>"....
            //}                  ;

            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = "Password",
            //    LoginPath = "<password - path>"....
            //}

            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = "TwoFactor",
            //    LoginPath = "<twofactor - path>",
            //    ....
            //}

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
