using EKO.ConnectFour.Domain.GridDomain.Contracts;

namespace EKO.ConnectFour.Domain.GridDomain;

/// <inheritdoc cref="IGridEvaluator"/>
public class GridEvaluator : IGridEvaluator
{
    public int CalculateScore(IGrid grid, DiscColor maximizingColor)
    {
        if (grid.WinningConnections.Count > 0)
        {
            var winningConnection = grid.WinningConnections[0];

            if (winningConnection.Color == maximizingColor)
            {
                return int.MaxValue;
            }
            else
            {
                return int.MinValue;
            }
        }

        var scoreMax = ScorePosition(grid, maximizingColor);

        var scoreMin = ScorePosition(grid, maximizingColor == DiscColor.Red ? DiscColor.Yellow : DiscColor.Red);

        return scoreMax - scoreMin;


        //throw new NotImplementedException();
    }

    private int ScorePosition(IGrid grid, DiscColor maximizingColor)
    {
        var score = 0;
        // Score center column
        var centerColumn = grid.NumberOfColumns / 2;
        var centerColumnDiscs = GetColumn(grid, centerColumn);
        score += centerColumnDiscs.Count(d => d?.Color == maximizingColor) * 3;
        // Score Horizontal
        for (var row = 0; row < grid.NumberOfRows; row++)
        {
            //var rowDiscs = grid.GetRow(row);
            var rowDiscs = GetRow(grid, row);
            for (var column = 0; column < grid.NumberOfColumns - 3; column++)
            {
                var window = rowDiscs.Skip(column).Take(4);
                score += EvaluateWindow(window, maximizingColor);
            }
        }
        // Score Vertical
        for (var column = 0; column < grid.NumberOfColumns; column++)
        {
            //var columnDiscs = grid.GetColumn(column);
            var columnDiscs = GetColumn(grid, column);
            for (var row = 0; row < grid.NumberOfRows - 3; row++)
            {
                var window = columnDiscs.Skip(row).Take(4);
                score += EvaluateWindow(window, maximizingColor);
            }
        }
        // Score Diagonal
        for (var row = 0; row < grid.NumberOfColumns - 3; row++)
        {
            for (var column = 0; column < grid.NumberOfColumns - 3; column++)
            {
                var diag1 = grid.Cells[row, column];
                var diag2 = grid.Cells[row + 1, column + 1];
                var diag3 = grid.Cells[row + 2, column + 2];
                
                if (row + 3 < grid.NumberOfRows)
                {
                    var diag4 = grid.Cells[row + 3, column + 3];
                }


                var window = new List<IDisc>
                {
                    //grid.Cells[row, column],
                    //grid.Cells[row + 1, column + 1],
                    //grid.Cells[row + 2, column + 2],
                    //grid.Cells[row + 3, column + 3]

                    diag1,
                    diag2,
                    diag3,
                };

                if (row + 3 < grid.NumberOfRows)
                {
                    var diag4 = grid.Cells[row + 3, column + 3];
                    window.Add(diag4);
                }
                score += EvaluateWindow(window, maximizingColor);
            }
        }
        for (var row = 0; row < grid.NumberOfRows - 3; row++)
        {
            for (var column = 0; column < grid.NumberOfColumns - 3; column++)
            {
                var window = new List<IDisc>
                {
                    grid.Cells[row + 3, column],
                    grid.Cells[row + 2, column + 1],
                    grid.Cells[row + 1, column + 2],
                    grid.Cells[row, column + 3]
                };
                score += EvaluateWindow(window, maximizingColor);
            }
        }
        return score;
    }

    private List<IDisc> GetColumn(IGrid grid, int column)
    {
        var columnDiscs = new List<IDisc>();
        for (var row = 0; row < grid.NumberOfRows; row++)
        {
            columnDiscs.Add(grid.Cells[row, column]);
        }
        return columnDiscs;
    }

    private List<IDisc> GetRow(IGrid grid, int row)
    {
        var rowDiscs = new List<IDisc>();
        for (var column = 0; column < grid.NumberOfColumns; column++)
        {
            rowDiscs.Add(grid.Cells[row, column]);
        }
        return rowDiscs;
    }

    private int EvaluateWindow(IEnumerable<IDisc> window, DiscColor maximizingColor)
    {
        var score = 0;

        var opponentColor = maximizingColor == DiscColor.Red ? DiscColor.Yellow : DiscColor.Red;

        var discCount = window.Count(d => d?.Color == maximizingColor);

        var opponentDiscCount = window.Count(d => d?.Color == opponentColor);

        var emptyDiscCount = window.Count(d => d == null);

        if (discCount == 4)
        {
            score += 100;
        }
        else if (discCount == 3 && emptyDiscCount == 1)
        {
            score += 5;
        }
        else if (discCount == 2 && emptyDiscCount == 2)
        {
            score += 2;
        }

        if (opponentDiscCount == 3 && emptyDiscCount == 1)
        {
            score -= 4;
        }

        return score;
    }
}