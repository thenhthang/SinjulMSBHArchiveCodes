using System.Linq;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace AuthorizeArea.SinjulMSBH.Conventions
{
    public class AuthorizeControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.RouteValues.Any()
                && controller.RouteValues.TryGetValue("area", out string adminAreaName)
                && adminAreaName.Equals("Admin"))
            {
                controller.Filters.Add(new AuthorizeFilter("AdminPolicy"));
            }
            if (controller.RouteValues.Any()
                && controller.RouteValues.TryGetValue("area", out string userAreaName)
                && userAreaName.Equals("User"))
            {
                controller.Filters.Add(new AuthorizeFilter("UserPolicy"));
            }
        }
    }
}
