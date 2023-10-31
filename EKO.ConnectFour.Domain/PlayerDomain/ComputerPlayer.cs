using EKO.ConnectFour.Domain.GameDomain.Contracts;
using EKO.ConnectFour.Domain.GridDomain;
using EKO.ConnectFour.Domain.GridDomain.Contracts;
using EKO.ConnectFour.Domain.PlayerDomain.Contracts;

namespace EKO.ConnectFour.Domain.PlayerDomain;

/// <inheritdoc cref="IPlayer"/>
public class ComputerPlayer : PlayerBase, IPlayer
{
    public ComputerPlayer(DiscColor color, int numberOfNormalDiscs, IGamePlayStrategy strategy) : base(Guid.NewGuid(), "Computer", color, numberOfNormalDiscs)
    {
        Color = color;
        NumberOfNormalDiscs = numberOfNormalDiscs;
        Strategy = strategy;
    }

    public Guid Id { get; }

    public string Name { get; }

    public DiscColor Color { get; }

    public int NumberOfNormalDiscs { get; private set; }

    public IGamePlayStrategy Strategy { get; }

    public IReadOnlyList<IDisc> SpecialDiscs { get; }

    public void AddDisc(DiscType discType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uses gameplay strategy to determine the best move to execute.
    /// </summary>
    /// <param name="game">The game (in its current state)</param>
    public IMove DetermineBestMove(IGame game)
    {
        throw new NotImplementedException();
    }

    public bool HasDisk(DiscType discType)
    {
        return discType == DiscType.Normal && NumberOfNormalDiscs > 0;
    }

    public void RemoveDisc(DiscType discType)
    {
        if (discType != DiscType.Normal)
            return;

        if (NumberOfNormalDiscs != 0)
        {
            NumberOfNormalDiscs--;
            return;
        }

        throw new InvalidOperationException("The player has no normal discs left.");
    }
}