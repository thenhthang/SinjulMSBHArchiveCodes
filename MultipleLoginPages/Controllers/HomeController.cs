using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MultipleLoginPages.Models;

namespace MultipleLoginPages.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) => _logger = logger;

     
        public IActionResult Index()
        {
            string controllerName = Request.RouteValues["controller"].ToString() + "Controller";
            string actionName = Request.RouteValues["action"].ToString();

            Assembly asm = Assembly.GetAssembly(typeof(Startup));

            Type myType =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .First(x => x.Name == controllerName)
            ;

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


        [Authorize(Roles = "Admin_Role")]
        public ContentResult AdminAccount() => Content(nameof(AdminAccount));

        [Authorize(Roles = "Customer_Role")]
        public ContentResult CustomerAccount() => Content(nameof(CustomerAccount));


        public ContentResult AdminAccountLogin() => Content(nameof(AdminAccountLogin));
        public ContentResult CustomerAccountLogin() => Content(nameof(CustomerAccountLogin));











        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
