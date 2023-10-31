using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GridDomain.Contracts;

namespace EKO.ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IGrid"/>
public class Grid : IGrid
{
    public Grid(GameSettings settings = null)
    {
        settings ??= new GameSettings();

        NumberOfRows = settings.GridRows;
        NumberOfColumns = settings.GridColumns;
        WinningConnectSize = settings.ConnectionSize;
        WinningConnections = new List<IConnection>();
        Cells = new IDisc[NumberOfRows, NumberOfColumns];
    }

    /// <summary>
    /// Creates a grid that is a copy of an other grid.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Grid(IGrid otherGrid)
    {
        //TODO: create a cells matrix and copy the values from the other grid
        //TODO: copy other property values
        throw new NotImplementedException();
    }

    public int NumberOfRows { get; }

    public int NumberOfColumns { get; }

    public int WinningConnectSize { get; }

    public IDisc[,] Cells { get; }

    public IReadOnlyList<IConnection> WinningConnections { get; private set; }

    public void PopOutDisc(IDisc disc, int column)
    {
        throw new NotImplementedException();
    }

    public void SlideInDisc(IDisc disc, int column)
    {
        for (int i = NumberOfRows - 1; i >= 0; i--)
        {
            if (Cells[i, column] == null)
            {
                Cells[i, column] = disc;

                CheckWin(i, column, disc);

                return;
            }
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Checks the Grid to see if there is a winning connection anywhere on the board.
    /// This method is called after every move.
    /// </summary>
    /// <param name="row">Row at which the <see cref="IDisc"/> was inserted.</param>
    /// <param name="col">Column at which the <see cref="IDisc"/> was inserted.</param>
    /// <param name="disc">Inserted <see cref="IDisc"/></param>
    private void CheckWin(int row, int col, IDisc disc)
    {
        // Check if the inserted disc has created a winning connection anywhere on the board.
        var winningConnections = new List<IConnection>
        {
            CheckHorizontalConnection(row, disc),
            CheckVerticalConnection(col, disc),
            CheckDiagonalTopLeftToBottomRightConnection(row, col, disc),
            CheckDiagonalBottomLeftToTopRightConnection(row, col, disc),
        };

        // Remove null values from the list.
        WinningConnections = winningConnections.Where(c => c != null).ToList();
    }

    /// <summary>
    /// Checks the Grid for a diagonal connection from top left to bottom right.
    /// </summary>
    /// <param name="row">Row of the inserted <see cref="IDisc"/></param>
    /// <param name="col">Column of the inserted <see cref="IDisc"/></param>
    /// <param name="disc">Inserted <see cref="IDisc"/></param>
    /// <returns>Returns a <see cref="IConnection"/> if one was present, otherwise returns null.</returns>
    private IConnection CheckDiagonalTopLeftToBottomRightConnection(int row, int col, IDisc disc)
    {
        int count = 0;

        // Find the starting point of the diagonal,
        // which is the top left corner of the grid or the column where the row is 0.
        while (row != 0 && col != 0)
        {
            row--;
            col--;
        }

        // Store the starting point of the diagonal.
        (int x, int y) startingPoint;

        if (row == 0)
        {
            startingPoint = (0, col);
        }
        else
        {
            startingPoint = (row, 0);
        }

        // Check the diagonal for a winning connection.
        while (row < NumberOfRows && col < NumberOfColumns)
        {
            // If the cell is empty, reset the count and continue to the next cell.
            if (Cells[row, col] == null)
            {
                count = 0;
                row++;
                col++;
                continue;
            }

            // If the cell is not empty and the color of the disc matches the color of the inserted disc,
            // increment the count.
            if (Cells[row, col].Color == disc.Color)
            {
                count++;

                // If this is the first found cell of a possible connection,
                // store the starting point of the connection.
                if (count == 1)
                {
                    startingPoint = (row, col);
                }
            }
            else
            {
                // If the color of the disc does not match the color of the inserted disc, reset the count.
                count = 0;
            }

            // If the count is equal to the winning connection size, a winning connection was found.
            if (count == WinningConnectSize)
            {
                // Return the winning connection.
                return new Connection(startingPoint.x, startingPoint.y, startingPoint.x + WinningConnectSize - 1, startingPoint.y + WinningConnectSize - 1, disc.Color);
            }

            // Continue to the next cell.
            row++;
            col++;
        }

        // Return null to indicate that no winning connection was found.
        return null;
    }

    /// <summary>
    /// Checks the Grid for a diagonal connection from bottom left to top right.
    /// </summary>
    /// <param name="row">Row of the inserted <see cref="IDisc"/></param>
    /// <param name="col">Column of the inserted <see cref="IDisc"/></param>
    /// <param name="disc">Inserted <see cref="IDisc"/></param>
    /// <returns>Returns a <see cref="IConnection"/> if one was present, otherwise returns null.</returns>
    private IConnection CheckDiagonalBottomLeftToTopRightConnection(int row, int col, IDisc disc)
    {
        int count = 0;

        // Find the starting point of the diagonal,
        // which is the bottom left corner of the grid or the column where the row is the last row.
        while (row != NumberOfRows - 1 && col != 0)
        {
            row++;
            col--;
        }

        // Store the starting point of the diagonal.
        (int x, int y) startingPoint;

        if (row == NumberOfRows - 1)
        {
            // If the starting point is the bottom left corner of the grid,
            // the starting point is the last row and the column where the row is the last row.
            startingPoint = (row, col);
        }
        else
        {
            // If the starting point is not the bottom left corner of the grid,
            startingPoint = (row, 0);
        }

        // Check the diagonal for a winning connection.
        while (row >= 0 && col < NumberOfColumns)
        {
            // If the cell is empty, reset the count and continue to the next cell.
            if (Cells[row, col] == null)
            {
                count = 0;
                row--;
                col++;
                continue;
            }

            // If the cell is not empty and the color of the disc matches the color of the inserted disc, increment the count.
            if (Cells[row, col].Color == disc.Color)
            {
                count++;

                // If this is the first found cell of a possible connection, store the starting point of the connection.
                if (count == 1)
                {
                    startingPoint = (row, col);
                }
            }
            else
            {
                count = 0;
            }

            // If the count is equal to the winning connection size, a winning connection was found.
            if (count == WinningConnectSize)
            {
                // Return the winning connection.
                return new Connection(startingPoint.x, startingPoint.y, startingPoint.x - WinningConnectSize + 1, startingPoint.y + WinningConnectSize - 1, disc.Color);
            }

            // Continue to the next cell.
            row--;
            col++;
        }

        // Return null to indicate that no winning connection was found.
        return null;
    }

    /// <summary>
    /// Checks the Grid for a horizontal connection.
    /// </summary>
    /// <param name="row">
    /// Row of the inserted <see cref="IDisc"/>,
    /// this row will be checked to see if there's a winning connection
    /// </param>
    /// <param name="disc">Newly inserted <see cref="IDisc"/></param>
    /// <returns>Returns a <see cref="IConnection"/> if one was present, otherwise returns null.</returns>
    private IConnection CheckHorizontalConnection(int row, IDisc disc)
    {
        int count = 0;

        // Store the starting column of the connection.
        int startingCol = 0;

        // Check the row for a winning connection.
        for (int i = 0; i < NumberOfColumns; i++)
        {
            // If the cell is empty, reset the count and continue to the next cell.
            if (Cells[row, i] == null)
            {
                count = 0;
                continue;
            }

            // If the cell is not empty and the color of the disc matches the color of the inserted disc, increment the count.
            if (Cells[row, i].Color == disc.Color)
            {
                count++;

                // If this is the first found cell of a possible connection, store the starting column of the connection.
                if (count == 1)
                {
                    startingCol = i;
                }
            }
            else
            {
                count = 0;
            }

            // If the count is equal to the winning connection size, a winning connection was found.
            if (count == WinningConnectSize)
            {
                // Return the winning connection.
                return new Connection(row, startingCol, row, startingCol + WinningConnectSize - 1, disc.Color);
            }
        }

        // Return null to indicate that no winning connection was found.
        return null;
    }

    /// <summary>
    /// Checks the Grid for a vertical connection.
    /// </summary>
    /// <param name="col">
    /// Column of the inserted <see cref="IDisc"/>,
    /// this column will be checked to see if there's a winning connection
    /// </param>
    /// <param name="disc">Newly inserted <see cref="IDisc"/></param>
    /// <returns>Returns a <see cref="IConnection"/> if one was present, otherwise returns null.</returns>
    private IConnection CheckVerticalConnection(int col, IDisc disc)
    {
        int count = 0;

        // Store the starting row of the connection.
        int startingRow = 0;

        // Check the column for a winning connection.
        for (int i = 0; i < NumberOfRows; i++)
        {
            // If the cell is empty, reset the count and continue to the next cell.
            if (Cells[i, col] == null)
            {
                count = 0;
                continue;
            }

            // If the cell is not empty and the color of the disc matches the color of the inserted disc, increment the count.
            if (Cells[i, col].Color == disc.Color)
            {
                count++;

                // If this is the first found cell of a possible connection, store the starting row of the connection.
                if (count == 1)
                {
                    startingRow = i;
                }
            }
            else
            {
                count = 0;
            }

            // If the count is equal to the winning connection size, a winning connection was found.
            if (count == WinningConnectSize)
            {
                // Return the winning connection.
                return new Connection(startingRow, col, startingRow + WinningConnectSize - 1, col, disc.Color);
            }
        }

        // Return null to indicate that no winning connection was found.
        return null;
    }
}