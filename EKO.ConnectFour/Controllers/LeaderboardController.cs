using EKO.ConnectFour.Api.Models;
using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain.GameDomain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EKO.ConnectFour.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaderboardController : ApiControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    /// <summary>
    /// Handles getting the leaderboard
    /// </summary>
    /// <returns><see cref="IEnumerable{T}"/> of <see cref="ILeaderboardEntry"/></returns>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ILeaderboardEntry>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetLeaderboard()
    {
        return Ok(_leaderboardService.GetLeaderboard());
    }

    /// <summary>
    /// Takes a game and checks to see who has won, updates the leaderboard entries accordingly
    /// </summary>
    /// <param name="gameId"><see cref="Guid"/> of the game to check</param>
    [HttpPost("evaluate-game/{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EvaluateGame(Guid gameId)
    {
        await _leaderboardService.EvaluateGameAsync(gameId);

        return Ok();
    }
}
