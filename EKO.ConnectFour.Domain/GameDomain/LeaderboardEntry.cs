using EKO.ConnectFour.Domain.GameDomain.Contracts;
using System.Text.Json.Serialization;

namespace EKO.ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="ILeaderboardEntry" />
public class LeaderboardEntry : ILeaderboardEntry
{
    public Guid UserId { get; init; }

    public required string Username { get; init; }

    [JsonInclude]
    public int Wins { get; private set; }

    [JsonInclude]
    public int Losses { get; private set; }

    public void IncrementWins() => Wins++;

    public void IncrementLosses() => Losses++;
}
