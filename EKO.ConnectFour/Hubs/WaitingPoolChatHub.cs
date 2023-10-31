using EKO.ConnectFour.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace EKO.ConnectFour.Api.Hubs;

public class WaitingPoolChatHub : Hub
{
    /// <summary>
    /// Key is the connection id. Value is the user.
    /// </summary>
    private static readonly Dictionary<string, ChatUserModel> _users = new();

    public async Task SendMessage(string senderName, string senderId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", senderName, senderId, message);
    }

    public async Task ConnectUser(string senderName, string senderId)
    {
        var user = new ChatUserModel
        {
            Username = senderName,
            UserId = senderId
        };

        _users.Add(Context.ConnectionId, user);
    }

    public async Task GetAllUsers()
    {
        await Clients.Caller.SendAsync("ReceiveAllUsers", _users.Values.ToArray());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _users.Remove(Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}
