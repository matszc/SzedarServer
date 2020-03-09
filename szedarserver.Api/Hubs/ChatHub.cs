using System.Net.WebSockets;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using szedarserver.Infrastructure.Models.HubModels;

namespace szedarserver.Api.Hubs
{
    public class ChatHub: Hub
    {
        public async Task JoinChat(string groupId, string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Join", name);
        }

        public async Task SendMessage(Message message)
        {
            await Clients.Group(message.GroupName).SendAsync("GetMessage",message);
        }

        public async Task LeaveChat(string groupId, string name)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Leave", name);
        }
    }
}