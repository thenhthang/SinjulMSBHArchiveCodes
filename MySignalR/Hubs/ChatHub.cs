using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace MySignalR.Hubs
{
    public class ChatHubPoco : Hub
    {
        public async Task Broadcast(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", new
            {
                Sender = Context.User.Identity.Name,
                Message = message
            });
        }

        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName)
                .SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName)
                .SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }

    public class ChatHubFull : Hub<IChatClient>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendBroadcastMessage(string message)
        {
            await Clients.All.ReceiveBroadcastMessage(new
            {
                Sender = Context.User.Identity.Name,
                Message = message
            });
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.ReceiveMessage(message);
        }

        [HubMethodName(nameof(SendPrivateMessage))]
        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).ReceivePrivateMessage("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");

            string connectionId = Context.ConnectionId;

            await base.OnDisconnectedAsync(exception);
        }


        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client .. !!!!");
        }
    }





    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessage2(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage2", new
            {
                Sender = Context.User?.Identity?.Name ?? "!User",
                Message = message
            });
        }


        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");

            string user = Context.User?.Identity?.Name ?? "SinjulMSBH";
            string connectionId = Context.ConnectionId;

            //await Clients.User(user).SendAsync("UserState", new
            //{
            //    UserId = user,
            //    ConnectionId = connectionId,
            //    Status = "Connected"
            //});

            await Clients.All.SendAsync("UserState", new
            {
                UserId = user,
                ConnectionId = connectionId,
                Status = "Connected"
            });

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");

            string user = Context.User?.Identity?.Name ?? "SinjulMSBH";
            string connectionId = Context.ConnectionId;

            //await Clients.User(user).SendAsync("UserState", new
            //{
            //    UserId = user,
            //    ConnectionId = connectionId,
            //    Status = "Disconnected"
            //});

            await Clients.All.SendAsync("UserState", new
            {
                UserId = user,
                ConnectionId = connectionId,
                Status = "Disconnected"
            });


            await base.OnDisconnectedAsync(exception);
        }

    }
}
