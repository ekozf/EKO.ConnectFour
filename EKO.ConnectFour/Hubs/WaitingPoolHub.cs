using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EKO.ConnectFour.Api.Hubs;

public class WaitingPoolHub : Hub
{
    private static readonly Dictionary<Guid, string> _users = new();
    private readonly ILiveWaitingPool _waitingPool;
    private readonly UserManager<User> _userManager;

    public WaitingPoolHub(ILiveWaitingPool waitingPool, UserManager<User> userManager)
    {
        _waitingPool = waitingPool;
        _userManager = userManager;
    }

    public async Task ConnectToWaitingPool(GameSettings gameSettings)
    {
        if (Context.User == null)
            throw new InvalidOperationException("User is not authenticated.");

        ArgumentNullException.ThrowIfNull(gameSettings);

        var currentUser = await _userManager.GetUserAsync(Context.User);

        _users.Add(currentUser!.Id, Context.ConnectionId);

        var gameId = _waitingPool.JoinLive(currentUser!, gameSettings);

        if (gameId == Guid.Empty)
            return;

        await Clients.Client(Context.ConnectionId).SendAsync("GameFound", gameId);
        await Clients.Client(Context.ConnectionId).SendAsync("GameFound", gameId);
    }

    public async Task DisconnectFromWaitingPool()
    {
        if (Context.User == null)
            return;

        var idClaimValue = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var result = Guid.TryParse(idClaimValue, out Guid userId) ? userId : Guid.Empty;

        _waitingPool.LeaveLive(result);
    }
}
