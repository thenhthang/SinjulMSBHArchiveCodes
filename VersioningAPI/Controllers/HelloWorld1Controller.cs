
using Microsoft.AspNetCore.Mvc;

namespace VersioningAPI.Controllers
{
    //[ApiController]
    //[Route("HelloWorld")]
    //[ApiVersion("1.0", Deprecated = true)]
    //public class HelloWorld1Controller : ControllerBase
    //{
    //    [HttpGet]
    //    public string Get() => "v1.0";
    //}


    // Will match "/v1.0/HelloWorld" and "/HelloWorld?api-version=1.0"
    [ApiController]
    [Route("HelloWorld")] // Support query string / header versioning
    [Route("v{version:apiVersion}/HelloWorld")] // Support path versioning
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HelloWorld1Controller : ControllerBase
    {
        public string Get() => "v1.0";
    }
}
