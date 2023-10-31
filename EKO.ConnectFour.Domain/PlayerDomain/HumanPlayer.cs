using EKO.ConnectFour.Domain.GridDomain;
using EKO.ConnectFour.Domain.GridDomain.Contracts;
using EKO.ConnectFour.Domain.PlayerDomain.Contracts;

namespace EKO.ConnectFour.Domain.PlayerDomain;

/// <inheritdoc cref="IPlayer"/>
public class HumanPlayer : PlayerBase, IPlayer
{
    public HumanPlayer(Guid userId, string name, DiscColor color, int numberOfNormalDiscs) : base(userId, name, color, numberOfNormalDiscs)
    {
        Id = userId;
        Name = name;
        Color = color;
        NumberOfNormalDiscs = numberOfNormalDiscs;
    }

    public Guid Id { get; }

    public string Name { get; }

    public DiscColor Color { get; }

    public int NumberOfNormalDiscs { get; private set; }

    public IReadOnlyList<IDisc> SpecialDiscs { get; }

    public void AddDisc(DiscType discType)
    {
        throw new NotImplementedException();
    }

    public bool HasDisk(DiscType discType)
    {
        return discType == DiscType.Normal && NumberOfNormalDiscs > 0;
    }

    public void RemoveDisc(DiscType discType)
    {
        if (discType == DiscType.Normal)
        {
            if (NumberOfNormalDiscs == 0)
            {
                throw new InvalidOperationException("The player has no normal discs left.");
            }
            else
            {
                NumberOfNormalDiscs--;
            }
        }
    }
}