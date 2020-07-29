using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MultipleLoginPages.Data;
using MultipleLoginPages.Models;

namespace MultipleLoginPages2
{
    public class Startup2
    {
        public Startup2(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        [AttributeUsage(AttributeTargets.Class)]
        public sealed class ILGAuthorizeAttribute : Attribute, IAuthorizationFilter
        {
            //private readonly ILGAuthorizeScheme _AuthenticationScheme;
            //public ILGAuthorizeAttribute(ILGAuthorizeScheme AuthenticationScheme)
            //{
            //    _AuthenticationScheme = AuthenticationScheme;
            //}
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var actionAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);

                    if (actionAttributes.Any(x => x is AllowAnonymousAttribute)) return;
                }

                if (filterContext != null)
                {
                    string url = filterContext.HttpContext.Request.Path;
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                    {
                        if (url.ToLower().StartsWith("/admin") && "_AuthenticationScheme".ToString().ToLower() == "admin")
                        {
                            var authenticateAdminResult = filterContext.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "UserId" && claim.Issuer.Equals("Admin", StringComparison.InvariantCultureIgnoreCase));
                            if (authenticateAdminResult == null)
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Home", action = "Index" }));
                        }
                        else
                        {
                            var authenticateSubscriberResult = filterContext.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "SubscriberId" && claim.Issuer.Equals("Subscriber", StringComparison.InvariantCultureIgnoreCase));
                            if (authenticateSubscriberResult == null)
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "Admin", controller = "Home", action = "Index" }));
                        }
                    }
                    else
                    {
                        if (url.ToLower().StartsWith("/admin"))
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "Admin", controller = "Account", action = "Login" }));
                        else
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Account", action = "CreateUsernamePassword" }));
                    }
                }
            }
        }


        public class CustomAuthorizeAttribute : TypeFilterAttribute
        {
            public CustomAuthorizeAttribute(string loginSchemeName) : base(typeof(CustomAuthorization))
            {
                Arguments = new object[] { "Admin" };
            }

            public CustomAuthorizeAttribute(Type type) : base(type)
            {
            }
        }
        //[AttributeUsage(AttributeTargets.Class)]
        public class CustomAuthorization : AuthorizeAttribute, IAuthorizationFilter/*, IAsyncAuthorizationFilter*/
        {
            public string LoginSchemeName { get; set; }

            public CustomAuthorization(string loginSchemeName) => LoginSchemeName = loginSchemeName;


            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (string.IsNullOrEmpty(LoginSchemeName))
                {
                    context.Result = new UnauthorizedResult();

                    return;
                }

                if (context != null)
                {
                    if (context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.HttpContext.Response.Headers.Add("LoginSchemeName", LoginSchemeName);

                        if (LoginSchemeName.Equals("Admin"))
                            context.Result = new RedirectToActionResult("Login", "Home", null);

                        if (LoginSchemeName.Equals("Customer"))
                            context.Result = new RedirectToActionResult("Login", "Home", null);

                        //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                        //context.HttpContext.Response.HttpContext.Features
                        //    .Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized .. !!!!";

                    }
                }
            }

            //public Task OnAuthorizationAsync(AuthorizationFilterContext context)
            //{
            //    return Task.CompletedTask;
            //}
        }


        public class MyAuthService : IAuthenticationService
        {
            public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }
        }

        public class MyAuthHandler : IAuthenticationHandler
        {
            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                string schemeName = scheme.Name;

                throw new NotImplementedException();
            }
        }


        public class MyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public MyAuthenticationHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock
            )
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                string scheme = Scheme.Name;

                return Task.FromResult(
                     AuthenticateResult.Success(
                         new AuthenticationTicket(
                            new ClaimsPrincipal(Context.User),
                            Scheme.Name
                        )
                    )
                );
            }

            protected override Task InitializeEventsAsync()
            {
                return base.InitializeEventsAsync();
            }

            protected override Task InitializeHandlerAsync()
            {
                return base.InitializeHandlerAsync();
            }

            protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
            {
                return base.HandleForbiddenAsync(properties);
            }

            protected override Task<object> CreateEventsAsync()
            {
                return base.CreateEventsAsync();
            }

            protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            {
                return base.HandleChallengeAsync(properties);
            }

            protected override string ResolveTarget(string scheme)
            {
                return base.ResolveTarget(scheme);
            }
        }

        public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
        {
            private IUrlHelperFactory _helper;
            private IActionContextAccessor _accessor;
            public CustomCookieAuthenticationEvents(IUrlHelperFactory helper, IActionContextAccessor accessor)
            {
                _helper = helper;
                _accessor = accessor;
            }
            public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
            {

                //using (var scope = context.HttpContext.RequestServices.CreateScope())
                //{
                //    IActionContextAccessor actionContextAccessor = 
                //        scope.ServiceProvider.GetRequiredService<IActionContextAccessor>();
                //}

                //var routeData = context.Request.HttpContext.GetRouteData();
                //RouteValueDictionary routeValues = new RouteValueDictionary();

                //Uri uri = new Uri(context.RedirectUri);
                //string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];

                //routeValues.Add(context.Options.ReturnUrlParameter, returnUrl + "focustab");
                //var urlHelper = _helper.GetUrlHelper(_accessor.ActionContext);
                //context.RedirectUri = UrlHelperExtensions.Action(urlHelper, "login", "account", routeValues);

                //var urlHelper = _helper.GetUrlHelper(_accessor.ActionContext);

                if (context.HttpContext.User.IsInRole("Admin_Role"))
                {
                    //context.RedirectUri = UrlHelperExtensions.Action(
                    //    helper: urlHelper, action: "AdminAccountLogin", controller: "Home"
                    //);

                    context.RedirectUri = "/Home/AdminAccountLogin";

                    return base.RedirectToLogin(context);
                }

                if (context.HttpContext.User.IsInRole("Customer_Role"))
                {
                    //context.RedirectUri = UrlHelperExtensions.Action(
                    //    helper: urlHelper, action: "CustomerAccountLogin", controller: "Home"
                    //);

                    context.RedirectUri = "/Home/CustomerAccountLogin";

                    return base.RedirectToLogin(context);
                }

                //context.RedirectUri = UrlHelperExtensions.Action(
                //    helper: urlHelper, action: "Login", controller: "Account", values: new { area = "Identity" }
                //);

                context.RedirectUri = "/Home/CustomerAccountLogin";
                //context.RedirectUri = "/Identity/Account/Login";

                return base.RedirectToLogin(context);
            }
        }


        public static class MultipleAutomaticAuthenticationDefaults
        {
            public const string AuthenticationScheme = "MultipleAutomaticAuthentication";
        }
        public class MultipleAutomaticAuthenticationOptions : AuthenticationSchemeOptions
        {
            public string[] Schemes { get; set; }
        }

        public class MultipleAutomaticAuthenticationHandler : AuthenticationHandler<MultipleAutomaticAuthenticationOptions>
        {
            public MultipleAutomaticAuthenticationHandler(
                IOptionsMonitor<MultipleAutomaticAuthenticationOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock) : base(options, logger, encoder, clock)
            {
            }
            protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                foreach (var scheme in Options.Schemes)
                {
                    var result = await Context.AuthenticateAsync(scheme);

                    if (result.Succeeded) return result;
                }

                return AuthenticateResult.Fail("Not authenticated .. !!!!");
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

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHttpContextAccessor();

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            //services.AddScoped<CustomCookieAuthenticationEvents>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "MultipleLoginPages";

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        RouteValueDictionary routeValues = context.Request.RouteValues;
                        string controllerName = routeValues["controller"].ToString() + "Controller";
                        string actionName = routeValues["action"].ToString();

                        Assembly asm = Assembly.GetAssembly(typeof(Startup2));

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

                #region Events

                //options.EventsType = typeof(CustomCookieAuthenticationEvents);

                //options.Events = new CookieAuthenticationEvents
                //{
                //    OnRedirectToLogin = opts =>
                //    {
                //        string scheme = opts.Scheme.Name;

                //        if (opts.HttpContext.User.IsInRole("Admin_Role"))
                //        {
                //            opts.RedirectUri = "/Home/AdminAccountLogin";

                //            return Task.CompletedTask;
                //        }

                //        if (opts.HttpContext.User.IsInRole("Customer_Role"))
                //        {
                //            opts.RedirectUri = "/Home/CustomerAccountLogin";

                //            return Task.CompletedTask;
                //        }

                //        opts.RedirectUri = "/Account/Identity/Login";
                //        return Task.CompletedTask;
                //    },

                //    OnSignedIn = context =>
                //    {
                //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                //          "OnSignedIn", context.Principal.Identity.Name);

                //        return Task.CompletedTask;
                //    },

                //    OnSigningOut = context =>
                //    {
                //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                //          "OnSigningOut", context.HttpContext.User.Identity.Name);

                //        return Task.CompletedTask;
                //    },

                //    OnValidatePrincipal = context =>
                //    {
                //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                //          "OnValidatePrincipal", context.Principal.Identity.Name);

                //        return Task.CompletedTask;
                //    },
                //};

                #endregion
            });


            //services.Configure<AuthorizationOptions>(options =>
            //{
            //    options.AddPolicy("Admin_Policy", policy => policy.RequireRole("ManageStore"));
            //    options.AddPolicy("Customer_Policy", policy => policy.RequireRole("Customer_Role"));
            //});

            //services.AddAuthentication(MultipleAutomaticAuthenticationDefaults.AuthenticationScheme)
            //   .AddScheme<MultipleAutomaticAuthenticationOptions, MultipleAutomaticAuthenticationHandler>(
            //      MultipleAutomaticAuthenticationDefaults.AuthenticationScheme, options =>
            //      {
            //          options.Schemes = new string[3] { "Cookie1", "Cookie2", "Cookie3" };
            //      })
            //   .AddCookie("Cookie1")
            //   .AddCookie("Cookie2")
            //   .AddCookie("Cookie3");



            //services.AddAuthentication(IdentityConstants.ApplicationScheme)
            //    .AddPolicyScheme("Admin_Scheme", null, _ => {})
            //    .AddPolicyScheme("Client_Scheme", null, null)
            //    .AddScheme<AuthenticationSchemeOptions, MyAuthHandler, MyAuthService>("Admin_Scheme", (_ , __) => { });
            //;

            //services.AddAuthentication(options =>
            //{
            //    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            //    options.DefaultScheme = "Admin_Scheme";
            //    options.DefaultChallengeScheme = "Admin_Scheme";
            //    options.DefaultAuthenticateScheme = "Admin_Scheme";
            //    options.DefaultForbidScheme = "Admin_Scheme";
            //    options.DefaultSignInScheme = "Admin_Scheme";
            //    options.DefaultSignOutScheme = "Admin_Scheme";

            //    //options.AddScheme("Admin_Scheme", opt =>
            //    //{

            //    //    opt.DisplayName = "Admin_Scheme";
            //    //    opt.HandlerType = typeof(MyAuthHandler);

            //    //});

            //    //options.AddScheme<MyAuthenticationHandler>(name: "Admin_Scheme", displayName: "Admin_Scheme");
            //    //options.AddScheme<MyAuthenticationHandler>(name: "Client_Scheme", displayName: "Client_Scheme");
            //});

            //services.AddAuthentication()
            //   .AddCookie("Admin_Scheme",
            //       options =>
            //       {
            //           options.LoginPath = "/Home/AdminAccountLogin";
            //           options.ExpireTimeSpan = TimeSpan.FromHours(1);
            //           options.Cookie.Name = "Admin";

            //       })
            //   .AddCookie("Client_Scheme",
            //       options =>
            //       {
            //           options.LoginPath = "/Home/CustomerAccountLogin";
            //           options.ExpireTimeSpan = TimeSpan.FromHours(1);
            //           options.Cookie.Name = "Client";

            //       })
            //;

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Admin_Policy",
            //        authBuilder =>
            //        {
            //            authBuilder.AddAuthenticationSchemes("Admin_Scheme");
            //            authBuilder.RequireRole("Admin_Role");
            //        });

            //    options.AddPolicy("Customer_Policy",
            //        authBuilder =>
            //        {
            //            authBuilder.AddAuthenticationSchemes("Client_Scheme");
            //            authBuilder.RequireRole("Customer_Role");
            //        });
            //});
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

            //app.UseCookiePolicy(new CookiePolicyOptions
            //{
            //    //CheckConsentNeeded = (ctx) =>
            //    //{
            //    //    return true;
            //    //}
            //});

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
