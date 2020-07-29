using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using MultipleLoginPages.Data;
using MultipleLoginPages.Models;

namespace MultipleLoginPages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
        {
            private readonly IUrlHelperFactory _helper;
            private readonly IActionContextAccessor _accessor;
            public CustomCookieAuthenticationEvents(IUrlHelperFactory helper, IActionContextAccessor accessor)
            {
                _helper = helper;
                _accessor = accessor;
            }

            public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
            {
                RouteData routeData = context.Request.HttpContext.GetRouteData();
                RouteValueDictionary routeValues = new RouteValueDictionary();

                Uri uri = new Uri(context.RedirectUri);
                string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];

                routeValues.Add(context.Options.ReturnUrlParameter, returnUrl + "focustab");
                IUrlHelper urlHelper = _helper.GetUrlHelper(_accessor.ActionContext);
                string redirectUri = UrlHelperExtensions.Action(urlHelper, "login", "account", routeValues);
                context.RedirectUri = redirectUri;


                RouteValueDictionary routeValues2 = context.Request.RouteValues;
                string controllerName = routeValues2["controller"].ToString() + "Controller";
                string actionName = routeValues2["action"].ToString();

                Assembly asm = Assembly.GetAssembly(typeof(Startup));

                Type myType =
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .First(x => x.Name == controllerName)
                ;

                IList<AuthorizeAttribute> controllerActionlistAutorizeAttributes = asm.GetTypes()
                        .Where(type => myType.IsAssignableFrom(type))
                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(m => !m.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any())
                        .Where(c => c.IsDefined(typeof(AuthorizeAttribute)))
                        .Select(x => new ControllerActions
                        {
                            Controller = x.DeclaringType.Name,
                            Action = x.Name,
                            AutorizeAttributes = x.GetCustomAttributes().OfType<AuthorizeAttribute>().ToList(),
                        })
                        .SingleOrDefault(_ => _.Controller == controllerName && _.Action == actionName)?
                        .AutorizeAttributes
                ;

                if (controllerActionlistAutorizeAttributes.Any(_ => _.Roles.Contains("Admin_Role")))
                    context.RedirectUri = "/Home/AdminAccountLogin";

                if (controllerActionlistAutorizeAttributes.Any(_ => _.Roles.Contains("Customer_Role")))
                    context.RedirectUri = "/Home/CustomerAccountLogin";

                return base.RedirectToLogin(context);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews(_ => _.EnableEndpointRouting = false);
            services.AddRazorPages();

            services.AddHttpContextAccessor();

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                ActionContext actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                IUrlHelperFactory factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "MultipleLoginPages";

                options.EventsType = typeof(CustomCookieAuthenticationEvents);

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        RouteValueDictionary routeValues = context.Request.RouteValues;
                        string controllerName = routeValues["controller"].ToString() + "Controller";
                        string actionName = routeValues["action"].ToString();

                        Assembly asm = Assembly.GetAssembly(typeof(Startup));

                        Type myType =
                            AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .First(x => x.Name == controllerName)
                        ;

                        IList<AuthorizeAttribute> controllerActionlistAutorizeAttributes = asm.GetTypes()
                                .Where(type => myType.IsAssignableFrom(type))
                                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                                .Where(m => !m.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any())
                                .Where(c => c.IsDefined(typeof(AuthorizeAttribute)))
                                .Select(x => new ControllerActions
                                {
                                    Controller = x.DeclaringType.Name,
                                    Action = x.Name,
                                    AutorizeAttributes = x.GetCustomAttributes().OfType<AuthorizeAttribute>().ToList(),
                                })
                                .SingleOrDefault(_ => _.Controller == controllerName && _.Action == actionName)?
                                .AutorizeAttributes
                        ;

                        if (controllerActionlistAutorizeAttributes.Any(_ => _.Roles.Contains("Admin_Role")))
                            context.RedirectUri = "/Home/AdminAccountLogin";

                        if (controllerActionlistAutorizeAttributes.Any(_ => _.Roles.Contains("Customer_Role")))
                            context.RedirectUri = "/Home/CustomerAccountLogin";

                        return Task.CompletedTask;
                    }
                };

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
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});

            app.UseMvcWithDefaultRoute();
        }
    }
}
