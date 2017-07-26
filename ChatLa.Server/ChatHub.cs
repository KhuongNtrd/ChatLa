using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace ChatLa.Server
{
    public class Room
    {
        public string Id { get; set; }
        public List<string> Users { get; set; }

        public Room()
        {
            Id = Guid.NewGuid().ToString();
            Users = new List<string>();
        }
    }
    public class ChatHub : Hub
    {
        public static List<Room> Rooms { get; set; } = new List<Room>();
        
        public override async Task OnConnected()
        {
            await base.OnConnected();

            var room = Rooms.FirstOrDefault(r => r.Users.Count < 2);

            if (room != null)
            {
                room.Users.Add(Context.ConnectionId);
            }
            else
            {
                room = new Room();
                room.Users.Add(Context.ConnectionId);
                Rooms.Add(room);
            }

            if (room.Users.Count == 2)
            {
                Clients.Clients(room.Users).Ready();
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public void Chat(string message)
        {
            var room = Rooms.FirstOrDefault(x => x.Users.Contains(Context.ConnectionId));
            if (room != null)
            {
                Clients.Clients(room.Users).Message(message, Context.ConnectionId);
            }
        }
    }
}
