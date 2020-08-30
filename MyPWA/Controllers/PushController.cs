
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
        public JsonResult GenerateKeys()
        {
            VapidDetails keys = VapidHelper.GenerateVapidKeys();

            var result = new
            {
                keys.PublicKey,
                keys.PrivateKey
            };

            JsonSerializerOptions jsonSerializerOptions =
                new JsonSerializerOptions { PropertyNamingPolicy = null }
            ;

            return new JsonResult(result, jsonSerializerOptions);
        }


        [HttpGet(nameof(SaveSubscription))]
        public async Task<OkObjectResult> SaveSubscription(
                                    [FromServices] IConfiguration configuration
        )
        {
            IConfigurationSection configVapidKeysOptionsSection =
                configuration.GetSection("ConfigVapidKeysOptions");

            var configVapidKeysOptionsBind = new
            {
                publicKey = configVapidKeysOptionsSection["PublicKey"],
                privateKey = configVapidKeysOptionsSection["PrivateKey"],
                subject = configVapidKeysOptionsSection["Subject"],
                endpoint = configVapidKeysOptionsSection["Endpoint"],
                keys = new
                {
                    p256dh = configVapidKeysOptionsSection["Keys:P256dh"],
                    auth = configVapidKeysOptionsSection["Keys:Auth"],
                },
                options = new
                {
                    gcmAPIKey = configVapidKeysOptionsSection["Options:gcmAPIKey"],
                    TTL = configVapidKeysOptionsSection["Options:TTL"],
                }
            };

            string payload =
                JsonSerializer.Serialize(
                    new
                    {
                        title = "SinjulMSBH",
                        message = "Hello from SinjulMSBH .. !!!!"
                    })
            ;

            PushSubscription pushSubscription =
                new PushSubscription(
                    configVapidKeysOptionsBind.endpoint,
                    configVapidKeysOptionsBind.keys.p256dh,
                    configVapidKeysOptionsBind.keys.auth)
            ;

            VapidDetails vapidDetails =
                new VapidDetails(
                    configVapidKeysOptionsBind.subject,
                    configVapidKeysOptionsBind.publicKey,
                    configVapidKeysOptionsBind.privateKey
                )
            ;

            await new WebPushClient().
                SendNotificationAsync(pushSubscription, payload, vapidDetails)
            ;


            return Ok(new { message = "Success, Send message from SinjulMSBH .. !!!!" });
        }
    }
}
