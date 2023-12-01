using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using ChatService.Model;
using ChatService.Model.Catalog;
using ChatService.Context;
using Microsoft.Extensions.Logging;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly ILogger<ChatHub> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ChatHub(
          IDictionary<string, UserConnection> connections,
          ILogger<ChatHub> logger,
          ApplicationDbContext dbContext)
        {
            //this._botUser = "Bot";
            this._connections = connections;
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.CourseId.ToString()).SendAsync("ReceiveMessage",  $"{userConnection.User} has left");
                SendUsersConnected(userConnection.CourseId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.CourseId.ToString());

            _connections[Context.ConnectionId] = userConnection;


            await SendUsersConnected(userConnection.CourseId);
        }

        public async Task SendMessageAndSaveToDb(string message)
        {
            if (!_connections.TryGetValue(Context.ConnectionId, out var userConnection))
            {
                userConnection = null;
            }
            else
            {
                await Clients.Group(userConnection.Room.ToString()).SendAsync("ReceiveMessage", userConnection.User, message);

                var chatMessage = new ChatMessage
                {
                    Message = message,
                    UserId = userConnection.User.ToString(),
                    CourseId = userConnection.Room.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                _dbContext.ChatMessages.Add(chatMessage);
                await _dbContext.SaveChangesAsync();

                userConnection = null;
            }
        }

        //public async Task SendMessageAndSaveToDb(string message)
        //{
        //    ChatHub chatHub = this;
        //    UserConnection userConnection;
        //    if (!chatHub._connections.TryGetValue(chatHub.Context.ConnectionId, out userConnection))
        //    {
        //        userConnection = (UserConnection)null;
        //    }
        //    else
        //    {
        //        await chatHub.Clients.Group(userConnection.Room.ToString()).SendAsync("ReceiveMessage", (object)userConnection.User, (object)message);
        //        ChatMessage entity = new ChatMessage()
        //        {
        //            Message = message,
        //            UserId = userConnection.User.ToString(),
        //            CourseId = userConnection.Room.ToString(),
        //            Timestamp = DateTime.UtcNow
        //        };
        //        chatHub._dbContext.ChatMessages.Add(entity);
        //        int num = await chatHub._dbContext.SaveChangesAsync();
        //        userConnection = (UserConnection)null;
        //    }
        //}

        public Task SendUsersConnected(Guid room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room.ToString()).SendAsync("UsersInRoom", users);
        }
    }
}
