using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

using MultipleLoginPages.Models;

namespace MultipleLoginPages.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IUrlHelperFactory _helper;
        private IActionContextAccessor _accessor;

        public HomeController(
            ILogger<HomeController> logger,
            IUrlHelperFactory helper,
            IActionContextAccessor accessor)
        {
            _logger = logger;
            _helper = helper;
            _accessor = accessor;
        }


        // explicit get
        [HttpGet]
        public IActionResult GetAction() => View();

        // extra info
        [HttpPost(Name = "some name", Order = 5)]
        public IActionResult PostAction() => View();

        [HttpPut]
        [HttpDelete]
        //[HttpPatch("my template", Name = "patch name", Order = 333)]
        public IActionResult MultiAction() => View();


        public static IEnumerable<Attribute> GetSupportedVerbsForAction<T>(
            Expression<Func<T, IActionResult>> expression)
            where T : Controller
        {
            //only consider a list of attributes
            var typesToCheck = new[] {
                typeof(AuthorizeAttribute),
                typeof(HttpGetAttribute),
                typeof(HttpPostAttribute),
                typeof(HttpPutAttribute),
                typeof(HttpDeleteAttribute),
                typeof(HttpPatchAttribute)
            };

            var method = ((MethodCallExpression)expression.Body).Method;

            var matchingAttributes = typesToCheck
                .Where(x => method.IsDefined(x))
                .ToList();

            //if the method doesn't have any of the attributes we're looking for,
            //assume that it does all verbs
            //if (!matchingAttributes.Any())
            //foreach (var verb in typesToCheck)
            //    yield return verb;

            //else, return all the attributes we did find
            foreach (var foundAttr in matchingAttributes)
                yield return method.GetCustomAttribute(foundAttr);
        }

        // implicit get
        public IActionResult Index()
        {
            //var adminAccount = GetSupportedVerbsForAction<HomeController>(x => x.AdminAccount()).ToList();
            //var customerAccount = GetSupportedVerbsForAction<HomeController>(x => x.CustomerAccount()).ToList();

            //var getVerbs = GetSupportedVerbsForAction<HomeController>(x => x.GetAction()).ToList();
            //var postVerbs = GetSupportedVerbsForAction<HomeController>(x => x.PostAction()).ToList();
            //var multiVerbs = GetSupportedVerbsForAction<HomeController>(x => x.MultiAction()).ToList();


            MethodInfo controllerMethod = typeof(HomeController).GetMethods().First();
            var attrs = controllerMethod.Attributes;
            var httpAttr = Attribute.GetCustomAttributes(typeof(HttpGetAttribute));
            var httpAttr2 = Attribute.GetCustomAttribute(controllerMethod, typeof(HttpGetAttribute));
            var httpAttr3 = Attribute.IsDefined(controllerMethod, typeof(HttpGetAttribute));

            var methods = typeof(HomeController).GetMethods();
            string infoString = "";
            foreach (var method in methods)
            {
                // Only public methods that are not constructors
                if (!method.IsConstructor && method.IsPublic)
                {
                    // Don't include inherited methods
                    if (method.DeclaringType == typeof(HomeController))
                    {

                        infoString += method.Name;

                        if (Attribute.IsDefined(method, typeof(HttpGetAttribute)))
                            infoString += " GET ";

                        if (Attribute.IsDefined(method, typeof(HttpPostAttribute)))
                            infoString += " POST ";

                        infoString += Environment.NewLine;
                    }
                }
            }


            string controllerName = Request.RouteValues["controller"].ToString() + "Controller";
            string actionName = Request.RouteValues["action"].ToString();

            Assembly asm = Assembly.GetAssembly(typeof(Startup));

            var ass =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .First(x => x.Name == controllerName)
            ;

            Type myType = default;
            try
            {
                // Since NoneSuch does not exist in this assembly, GetType throws a TypeLoadException.
                myType = Type.GetType(typeName: "MultipleLoginPages.Controllers.HomeController", throwOnError: true);
                Console.WriteLine("The full name is {0}.", myType.FullName);
            }
            catch (TypeLoadException e)
            {
                Console.WriteLine("{0}: Unable to load type NoneSuch", e.GetType().Name);
            }

            var controlleractionlist = asm.GetTypes()
                    .Where(type => myType.IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any())
                    .Where(c => c.IsDefined(typeof(AuthorizeAttribute)))
                    .Select(x => new
                    {
                        Controller = x.DeclaringType.Name,
                        Action = x.Name,
                        ReturnType = x.ReturnType.Name,
                        AutorizeAttributes = x.GetCustomAttributes().OfType<AuthorizeAttribute>().ToList(),
                        Attributes = string.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))
                    })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            IList<ControllerActions> list = new List<ControllerActions>();

            foreach (var item in controlleractionlist)
            {
                list.Add(new ControllerActions()
                {
                    Controller = item.Controller,
                    Action = item.Action,
                    Attributes = item.Attributes,
                    ReturnType = item.ReturnType,
                    AutorizeAttributes = item.AutorizeAttributes,
                });
            }

            IList<AuthorizeAttribute> autorizeAttributes =
                list.SingleOrDefault(_ => _.Controller == controllerName && _.Action == "AdminAccount").AutorizeAttributes
            ;

            return View(list);
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


        [Authorize(Roles = "Admin_Role")]
        [Authorize(Roles = "Admin_Role2", Policy = "Admin_Policy")]
        //[CustomAuthorize(loginSchemeName: "Admin")]
        //[Authorize(AuthenticationSchemes = "Admin_Scheme")]
        //[Authorize(policy: "Admin_Policy", AuthenticationSchemes = "Admin_Scheme")]
        //[Authorize(Roles = "Admin_Role", AuthenticationSchemes = "Admin_Scheme")]
        public ContentResult AdminAccount() => Content(nameof(AdminAccount));

        [Authorize(Roles = "Customer_Role")]
        [Authorize(Roles = "Customer_Role2", Policy = "Customer_Policy")]
        //[Authorize(AuthenticationSchemes = "Client_Scheme")]
        //[Authorize(policy: "Customer_Policy", AuthenticationSchemes = "Client_Scheme")]
        //[Authorize(Roles = "Customer_Role", AuthenticationSchemes = "Client_Scheme")]
        public ContentResult CustomerAccount() => Content(nameof(CustomerAccount));


        public ContentResult AdminAccountLogin() => Content(nameof(AdminAccountLogin));
        public ContentResult CustomerAccountLogin() => Content(nameof(CustomerAccountLogin));
    }
}
