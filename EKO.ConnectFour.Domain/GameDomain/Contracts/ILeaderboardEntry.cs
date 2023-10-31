namespace EKO.ConnectFour.Domain.GameDomain.Contracts;

/// <summary>
/// An entry in the leaderboard
/// </summary>
public interface ILeaderboardEntry
{
    /// <summary>
    /// The Id of an User, used for identification purposes
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// The username of an User, used for display purposes
    /// </summary>
    string Username { get; }

    /// <summary>
    /// Amount of times the user has won a game
    /// </summary>
    int Wins { get; }

    /// <summary>
    /// Amount of times the user has lost a game
    /// </summary>
    int Losses { get; }

    /// <summary>
    /// Increment the amount of wins by 1
    /// </summary>
    void IncrementWins();

    /// <summary>
    /// Increment the amount of losses by 1
    /// </summary>
    void IncrementLosses();
}
