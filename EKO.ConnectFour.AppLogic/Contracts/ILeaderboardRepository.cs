using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.AppLogic.Contracts;

/// <summary>
/// Loads, stores, and updates the leaderboard in server memory
/// </summary>
public interface ILeaderboardRepository
{
    /// <summary>
    /// Gets the leaderboard
    /// </summary>
    /// <returns>All leaderboard entries</returns>
    IReadOnlyList<ILeaderboardEntry> GetLeaderboard();

    /// <summary>
    /// Updates the leaderboard with the given winner and loser
    /// </summary>
    /// <param name="winnerUserId"><see cref="Guid"/> of the user who has won the game</param>
    /// <param name="loserUserId"><see cref="Guid"/> of the user who has lost the game</param>
    /// <param name="gameId"><see cref="Guid"/> of the game to update the entries for</param>
    Task UpdateLeaderboardEntries(Guid winnerUserId, Guid loserUserId, Guid gameId);

    /// <summary>
    /// Gets the leaderboard entry for the given user
    /// </summary>
    /// <param name="userId"><see cref="Guid"/> of the user to search</param>
    /// <returns><see cref="ILeaderboardEntry"/> that was found, otherwise returns null.</returns>
    ILeaderboardEntry? GetLeaderboardEntryByUserId(Guid userId);

    /// <summary>
    /// Adds the given entry to the leaderboard
    /// </summary>
    /// <param name="entry"><see cref="ILeaderboardEntry"/> to add to the leaderboard</param>
    Task AddUserLeaderboardEntry(ILeaderboardEntry entry);
}
