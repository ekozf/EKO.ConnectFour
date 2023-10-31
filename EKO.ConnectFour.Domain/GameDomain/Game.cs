using EKO.ConnectFour.Domain.GameDomain.Contracts;
using EKO.ConnectFour.Domain.GridDomain;
using EKO.ConnectFour.Domain.GridDomain.Contracts;
using EKO.ConnectFour.Domain.PlayerDomain.Contracts;

namespace EKO.ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGame"/>
internal class Game : IGame
{
    public Guid Id { get; }

    public IPlayer Player1 { get; }

    public IPlayer Player2 { get; }

    public Guid PlayerToPlayId { get; private set; }

    public IGrid Grid { get; }

    public bool Finished
    {
        get
        {
            if (Grid.WinningConnections.Count > 0) return true;
            if (Player1.NumberOfNormalDiscs == 0 || Player2.NumberOfNormalDiscs == 0) return true;
            if (!GetPlayerById(PlayerToPlayId).HasDisk(DiscType.Normal)) return true;

            return false;
        }
    }

    public bool PopOutAllowed { get; }

    public Game(IPlayer player1, IPlayer player2, IGrid grid, bool popOutAllowed = false)
    {
        ArgumentNullException.ThrowIfNull(player1);

        ArgumentNullException.ThrowIfNull(player2);

        ArgumentNullException.ThrowIfNull(grid);

        Id = Guid.NewGuid();
        Player1 = player1;
        Player2 = player2;
        PlayerToPlayId = player1.Id;
        Grid = grid;
    }

    /// <summary>
    /// Creates a game that is a copy of an other game.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Game(IGame otherGame)
    {
        //TODO: make a copy of the players
        //TODO: make a copy of the grid
        //TODO: initialize the properties with the copies

        Id = otherGame.Id;
        Player1 = otherGame.Player1;
        Player2 = otherGame.Player2;
        PlayerToPlayId = otherGame.PlayerToPlayId;
        Grid = otherGame.Grid;
        PopOutAllowed = otherGame.PopOutAllowed;

        // throw new NotImplementedException();
    }

    public IReadOnlyList<IMove> GetPossibleMovesFor(Guid playerId)
    {
        if (playerId != PlayerToPlayId) return new List<IMove>();

        var player = GetPlayerById(playerId);

        if (!player.HasDisk(DiscType.Normal)) return new List<IMove>();

        var possibleMoves = new List<IMove>();

        // Check each column from left to right
        for (int col = 0; col < Grid.NumberOfColumns; col++)
        {
            // Check each row from bottom to top
            for (int row = Grid.NumberOfRows - 1; row >= 0; row--)
            {
                var cell = Grid.Cells[row, col];

                // If the cell is not empty, check the next row
                if (cell != null) continue;

                // If the cell is empty, check if we already found a cell in this column
                if (possibleMoves.Any(x => x.Column == col)) break;

                // If the cell is empty and we didn't find a cell in this column yet, add the move
                possibleMoves.Add(new Move(col));
            }
        }

        return possibleMoves;
    }

    public void ExecuteMove(Guid playerId, IMove move)
    {
        IPlayer player = GetPlayerById(playerId);

        if (Finished)
        {
            throw new InvalidOperationException("The game has already finished.");
        }

        if (!player.HasDisk(move.DiscType))
        {
            throw new InvalidOperationException("The player does not exist.");
        }

        if (playerId != PlayerToPlayId)
        {
            throw new InvalidOperationException("It's not your turn.");
        }

        if (player.HasDisk(move.DiscType))
        {
            player.RemoveDisc(move.DiscType);

            var disc = new Disc(move.DiscType, player.Color);

            Grid.SlideInDisc(disc, move.Column);

            PlayerToPlayId = GetOpponent(playerId).Id;
        }
    }


    public IPlayer GetPlayerById(Guid playerId)
    {
        if (Player1.Id == playerId) return Player1;
        if (Player2.Id == playerId) return Player2;

        throw new InvalidOperationException($"The player with id {playerId} is not part of this game.");
    }

    public IPlayer GetOpponent(Guid playerId)
    {
        if (Player1.Id == playerId) return Player2;
        if (Player2.Id == playerId) return Player1;

        throw new InvalidOperationException($"The player with id {playerId} is not part of this game.");
    }
}