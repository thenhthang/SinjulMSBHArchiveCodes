using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using BlazorPwaTodo.Shared;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WebPush;

namespace BlazorPwaTodo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotificationController : Controller
    {
        //private readonly PizzaStoreContext _db;

        //public NotificationsController(PizzaStoreContext db)
        //{
        //    _db = db;
        //}

        private static async Task SendNotificationAsync(TodoItem todoItem, NotificationSubscription subscription, string message)
        {
            // For a real application, generate your own
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"mytodoitem/{todoItem.Title}",
                });

                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error sending push notification: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
        {
            // We're storing at most one subscription per user, so delete old ones.
            // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
            var userId = GetUserId();
            //var oldSubscriptions = _db.NotificationSubscriptions.Where(e => e.UserId == userId);
            //_db.NotificationSubscriptions.RemoveRange(oldSubscriptions);

            // Store new subscription
            subscription.UserId = userId;
            //_db.NotificationSubscriptions.Attach(subscription);

            //await _db.SaveChangesAsync();


            // In a realistic case, some other backend process would track
            // order delivery progress and send us notifications when it
            // changes. Since we don't have any such process here, fake it.
            //await Task.Delay(TimeSpan.FromSeconds(13));
            await SendNotificationAsync(
                new TodoItem { Title = "SinjulMSBH", IsDone = true },
                subscription, "Your order has been dispatched!")
            ;

            //await Task.Delay(TimeSpan.FromSeconds(13));
            //await SendNotificationAsync(
            //    new TodoItem { Title = "JackSlater", IsDone = false },
            //    subscription, "Your order is now delivered. Enjoy!")
            //;

            return subscription;
        }

        private string GetUserId() => 
            HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
