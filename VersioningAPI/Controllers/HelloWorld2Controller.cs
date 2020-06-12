
using Microsoft.AspNetCore.Mvc;

namespace VersioningAPI.Controllers
{
    //[ApiController]
    //[Route("HelloWorld")]
    //[ApiVersion("2.0")]
    //public class HelloWorld2Controller : ControllerBase
    //{
    //    [HttpGet]
    //    public string Get() => "v2.0";
    //}

    // 👇 Declare both versions
    [ApiVersion("2.0")]
    [ApiVersion("2.1")]
    [ApiController, Route("HelloWorld")]
    public class HelloWorld2Controller : ControllerBase
    {
        // Common to v2.0 and v2.1
        // You can use HttpContext.GetRequestedApiVersion to get the matched version
        [HttpPost]
        public string Post() => "v" + HttpContext.GetRequestedApiVersion();

        // 👇 Map to v2.0
        [HttpGet, MapToApiVersion("2.0")]
        public string Get() => "v2.0";

        // 👇 Map to v2.1
        [HttpGet, MapToApiVersion("2.1")]
        public string Get2_1() => "v2.1";
    }
}
