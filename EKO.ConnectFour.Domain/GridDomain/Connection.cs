using EKO.ConnectFour.Domain.GridDomain.Contracts;

namespace EKO.ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IConnection"/>
public class Connection : IConnection
{
    public static Connection Empty
    {
        get
        {
            return new Connection();
        }
    }

    public GridCoordinate From { get; }

    public GridCoordinate To { get; }

    public int Size { get; }

    public DiscColor Color { get; }

    public Connection(int rowFrom, int columnFrom, int rowTo, int columnTo, DiscColor color)
    {
        From = new GridCoordinate(rowFrom, columnFrom);
        To = new GridCoordinate(rowTo, columnTo);
        Color = color;
        Size = Math.Max(Math.Abs(rowFrom - rowTo), Math.Abs(columnFrom - columnTo)) + 1;
    }

    private Connection()
    {
        Color = DiscColor.Red;
        Size = 0;
        From = GridCoordinate.Empty;
        To = GridCoordinate.Empty;
    }
}