using System.Threading.Tasks;

namespace MySignalR.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
        Task ReceiveMessage(string user, string message);
        Task ReceiveBroadcastMessage(object user);
        Task ReceivePrivateMessage(string user, string message);
        Task SendMessageToCaller(string message);
        Task SendMessageToGroup(string message);
    }
}
