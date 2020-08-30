
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

using WebPush;

namespace MyPWA.Controllers
{
    [ApiController]
    public class PushController : ControllerBase
    {
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


        [HttpGet(nameof(SaveSubscription))]
        public async Task<OkObjectResult> SaveSubscription(
                                    [FromServices] IConfiguration configuration
        )
        {
            string payload =
                JsonSerializer.Serialize(
                    new
                    {
                        title = "SinjulMSBH",
                        message = "Hello from SinjulMSBH .. !!!!"
                    })
            ;

            string vapidPublicKey = configuration["VapidKeys:PublicKey"];
            string vapidPrivateKey = configuration["VapidKeys:PrivateKey"];
            string vapidP256dh = configuration["VapidKeys:P256dh"];
            string vapidAuth = configuration["VapidKeys:Auth"];
            string vapidEndpoint = configuration["VapidKeys:Endpoint"];

            PushSubscription pushSubscription =
                new PushSubscription(vapidEndpoint, vapidP256dh, vapidAuth)
            ;

            VapidDetails vapidDetails =
                new VapidDetails(
                    "mailto:Sinjul.MSBH@Yahoo.Com",
                    vapidPublicKey,
                    vapidPrivateKey);

            WebPushClient webPushClient = new WebPushClient();
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);

            return Ok(new { message = "success" });
        }
    }
}
