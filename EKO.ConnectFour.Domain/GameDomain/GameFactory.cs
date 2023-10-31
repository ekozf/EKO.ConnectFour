using EKO.ConnectFour.Domain.GameDomain.Contracts;
using EKO.ConnectFour.Domain.GridDomain;
using EKO.ConnectFour.Domain.PlayerDomain;
using EKO.ConnectFour.Domain.PlayerDomain.Contracts;

namespace EKO.ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGameFactory"/>
public class GameFactory : IGameFactory
{
    private readonly IGamePlayStrategy gamePlayStrategy;

    public GameFactory(IGamePlayStrategy gamePlayStrategy)
    {
        this.gamePlayStrategy = gamePlayStrategy;
    }

    public IGame CreateNewSinglePlayerGame(GameSettings settings, User user)
    {
        var grid = new Grid(settings);

        var player1 = new HumanPlayer(user.Id, user.NickName, DiscColor.Red, settings.GridRows * settings.GridColumns / 2);

        var player2 = new ComputerPlayer(DiscColor.Yellow, settings.GridRows * settings.GridColumns / 2, gamePlayStrategy);

        return new Game(player1, player2, grid);
    }

    public IGame CreateNewTwoPlayerGame(GameSettings settings, User user1, User user2)
    {
        if (user1.Id == user2.Id) throw new InvalidOperationException();

        var grid = new Grid(settings);

        var player1 = new HumanPlayer(user1.Id, user1.NickName, DiscColor.Red, settings.GridRows * settings.GridColumns / 2);
        var player2 = new HumanPlayer(user2.Id, user2.NickName, DiscColor.Yellow, settings.GridRows * settings.GridColumns / 2);

        return new Game(player1, player2, grid);
    }
}