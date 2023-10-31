using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Common;
using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace EKO.ConnectFour.AppLogic;

/// <inheritdoc cref="ILeaderboardService" />
public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IGameRepository _gameRepository;
    private readonly UserManager<User> _userManager;

    public LeaderboardService(ILeaderboardRepository leaderboardRepository, IGameRepository gameRepository, UserManager<User> userManager)
    {
        _leaderboardRepository = leaderboardRepository;
        _gameRepository = gameRepository;
        _userManager = userManager;
    }

    public async Task EvaluateGameAsync(Guid gameId)
    {
        var game = _gameRepository.GetById(gameId);

        if (!game.Finished)
        {
            throw new InvalidOperationException("Cannot evaluate an ongoing game!");
        }

        // Only checking the first connection is enough
        var winningColor = game.Grid.WinningConnections[0].Color;

        // Determine the winner and loser
        var winner = game.Player1.Color == winningColor ? game.Player1.Id : game.Player2.Id;
        var loser = game.Player1.Color == winningColor ? game.Player2.Id : game.Player1.Id;

        // Get the leaderboard entries for the winner and loser
        var winnerEntry = _leaderboardRepository.GetLeaderboardEntryByUserId(winner);
        var loserEntry = _leaderboardRepository.GetLeaderboardEntryByUserId(loser);

        if (winnerEntry == null)
        {
            // If the winner doesn't have an entry yet, create one
            var user = await _userManager.FindByIdAsync(winner.ToString()) ?? throw new DataNotFoundException();

            winnerEntry = new LeaderboardEntry
            {
                UserId = winner,
                Username = user.NickName,
            };

            await _leaderboardRepository.AddUserLeaderboardEntry(winnerEntry);
        }

        if (loserEntry == null)
        {
            // If the loser doesn't have an entry yet, create one
            var user = await _userManager.FindByIdAsync(loser.ToString()) ?? throw new DataNotFoundException();

            loserEntry = new LeaderboardEntry
            {
                UserId = loser,
                Username = user.NickName,
            };

            await _leaderboardRepository.AddUserLeaderboardEntry(loserEntry);
        }

        // Update the leaderboard entries
        await _leaderboardRepository.UpdateLeaderboardEntries(winner, loser, gameId);
    }

    public IEnumerable<ILeaderboardEntry> GetLeaderboard()
    {
        return _leaderboardRepository.GetLeaderboard().OrderByDescending(x => x.Wins);
    }
}
