using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;
using System.Text.Json;

namespace EKO.ConnectFour.Infrastructure.Storage;

/// <inheritdoc cref="ILeaderboardRepository" />
/// <remarks>
/// Note that we are using a file to save our leaderboard,
/// we would've liked to add it with EntityFramework, but didn't know if we were allowed to edit the database.
/// </remarks>
public class InMemoryLeaderboardRepository : ILeaderboardRepository
{
    /// <summary>
    /// This is the list of all leaderboard entries, loaded from and saved to 'LEADERBOARD_FILE_NAME'
    /// </summary>
    private readonly List<ILeaderboardEntry> _leaderboardEntries;

    /// <summary>
    /// All the games that have been evaluated, so we don't evaluate them again.
    /// Doesn't get saved to the file, because it's not necessary.
    /// </summary>
    private readonly List<Guid> _checkedGames;

    /// <summary>
    /// File name of the leaderboard file for development
    /// </summary>
    private const string LEADERBOARD_FILE_NAME_DEV = "leaderboard.json";

    /// <summary>
    /// File name of the leaderboard file for deployment
    /// </summary>
    private const string LEADERBOARD_FILE_NAME_PROD = "dbs/leaderboard.json";

#if DEBUG
    private const string LEADERBOARD_FILE_NAME = LEADERBOARD_FILE_NAME_DEV;
#else
    private const string LEADERBOARD_FILE_NAME = LEADERBOARD_FILE_NAME_PROD;
#endif

    public InMemoryLeaderboardRepository()
    {
        _leaderboardEntries = new List<ILeaderboardEntry>();
        _checkedGames = new List<Guid>();

        // Load the data for the leaderboard from the 'LEADERBOARD_FILE_NAME' file
        LoadLeaderboard();
    }

    public async Task UpdateLeaderboardEntries(Guid winnerUserId, Guid loserUserId, Guid gameId)
    {
        if (_checkedGames.Contains(gameId))
            return;

        _checkedGames.Add(gameId);

        // We assume that the users exist, otherwise we wouldn't have been able to get here
        var winnerEntry = GetLeaderboardEntryByUserId(winnerUserId)!;
        var loserEntry = GetLeaderboardEntryByUserId(loserUserId)!;

        // Update the wins and losses
        winnerEntry.IncrementWins();
        loserEntry.IncrementLosses();

        // Write the file
        await SaveLeaderboard();
    }

    public async Task AddUserLeaderboardEntry(ILeaderboardEntry entry)
    {
        // Add the new entry and save the file
        _leaderboardEntries.Add(entry);

        await SaveLeaderboard();
    }

    public IReadOnlyList<ILeaderboardEntry> GetLeaderboard()
    {
        return _leaderboardEntries;
    }

    public ILeaderboardEntry? GetLeaderboardEntryByUserId(Guid userId)
    {
        return _leaderboardEntries.Find(entry => entry.UserId == userId);
    }

    private async Task SaveLeaderboard()
    {
        // Save and write the file
        var json = JsonSerializer.Serialize(_leaderboardEntries);

        // Let's hope this doesn't throw an exception and that concurrent writes don't happen
        await File.WriteAllTextAsync(LEADERBOARD_FILE_NAME, json);
    }

    private void LoadLeaderboard()
    {
        if (!File.Exists(LEADERBOARD_FILE_NAME))
            return;

        // Hope it works
        var json = File.ReadAllText(LEADERBOARD_FILE_NAME);

        // Assuming that the file is valid, do not want to handle exceptions right now
        var data = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json)!;

        _leaderboardEntries.AddRange(data);
    }
}
