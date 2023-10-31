using EKO.ConnectFour.Domain.GridDomain.Contracts;

namespace EKO.ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IDisc"/>
public class Disc : IDisc
{
    public Disc(DiscType type, DiscColor color)
    {
        Type = type;
        Color = color;
    }

    public DiscType Type { get; }

    public DiscColor Color { get; }
}