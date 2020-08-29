
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using WebPush;

namespace MyPWA.Controllers
{
    [ApiController]
    public class PushController : ControllerBase
    {
        [HttpPost(nameof(SaveSubscription))]
        public OkObjectResult SaveSubscription(
            string subscription,
            [FromServices] IConfiguration configuration)
        {
            string payload = Request.Form["payload"].ToString();
            string device = subscription;

            string vapidPublicKey = configuration.GetSection("VapidKeys:PublicKey")["PublicKey"];
            string vapidPrivateKey = configuration.GetSection("VapidKeys:PrivateKey")["PrivateKey"];

            PushSubscription pushSubscription =
                new PushSubscription(
                    "device.PushEndpoint",
                    "device.PushP256DH",
                    "device.PushAuth");

            VapidDetails vapidDetails =
                new VapidDetails(
                    "mailto:Sinjul.MSBH@Yahoo.Com",
                    vapidPublicKey,
                    vapidPrivateKey);

            WebPushClient webPushClient = new WebPushClient();
            webPushClient.SendNotification(pushSubscription, payload, vapidDetails);

            return Ok(new { message = "success" });
        }

        [HttpGet(nameof(GenerateKeys))]
        public OkObjectResult GenerateKeys()
        {
            VapidDetails keys = VapidHelper.GenerateVapidKeys();

            var result = new
            {
                keys.PublicKey,
                keys.PrivateKey
            };

            return Ok(result);
        }
    }
}
