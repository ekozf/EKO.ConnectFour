using EKO.ConnectFour.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace EKO.ConnectFour.Api.Hubs;

public class GameChatHub : Hub
{
    private static readonly Dictionary<string, GameChatUserModel> _users = new();

    public async Task SendMessage(string sender, string senderId, string message, string gameId)
    {
        await Clients.Group(gameId).SendAsync("ReceiveMessage", sender, senderId, message);
    }

    public async Task ConnectUser(string registrarName, string registrarId, string gameId)
    {
        var user = new GameChatUserModel
        {
            Username = registrarName,
            UserId = registrarId,
            GameId = Guid.Parse(gameId)
        };

        var added = _users.TryAdd(Context.ConnectionId, user);

        if (added)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _users.Remove(Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}
