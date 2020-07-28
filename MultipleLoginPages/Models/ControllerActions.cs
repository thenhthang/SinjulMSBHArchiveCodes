using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

namespace MultipleLoginPages.Models
{
    public class ControllerActions
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Attributes { get; set; }
        public string ReturnType { get; set; }
        public IList<AuthorizeAttribute> AutorizeAttributes { get; set; }
    }
}
