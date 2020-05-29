using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPwaTodo.Shared;

namespace BlazorPwaTodo.Client
{
    public class Client
    {
        private readonly HttpClient httpClient;

        public Client(HttpClient httpClient) => this.httpClient = httpClient;

        public async Task SubscribeToNotifications(NotificationSubscription subscription)
        {
            var response = await httpClient.PostAsJsonAsync("api/notification/subscribe", subscription);

            response.EnsureSuccessStatusCode();
        }
    }

}
