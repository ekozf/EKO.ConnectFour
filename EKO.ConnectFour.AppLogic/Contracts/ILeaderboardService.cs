using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.AppLogic.Contracts;

/// <summary>
/// Service to manipulate the leaderboard
/// </summary>
public interface ILeaderboardService
{
    /// <summary>
    /// Gets the leaderboard, ordered by wins descending
    /// </summary>
    IEnumerable<ILeaderboardEntry> GetLeaderboard();

    /// <summary>
    /// Checks the game and updates the leaderboard accordingly
    /// </summary>
    /// <param name="gameId"><see cref="Guid"/> of the game to check</param>
    Task EvaluateGameAsync(Guid gameId);
}
