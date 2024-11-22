using System;
using System.Text;

namespace Codewars.Training.ConveysGameOfLife;

public static class Kata
{
    ///Any live cell with fewer than two live neighbours dies, as if caused by the underpopulation.
    ///Any live cell with more than three live neighbours dies, as if by overcrowding.
    ///Any live cell with two or three live neighbours lives on to the next generation.
    ///Any dead cell with exactly three live neighbours becomes a live cell.
    public static int[,] GetGeneration(int[,] cells, int generation)
    {
        var result = cells;
        for (var gen = 0; gen < generation; ++gen)
        {
            var extendedCells = ExtrudeCells(result);
            var rows = extendedCells.GetLength(0);
            var cols = extendedCells.GetLength(1);
            var newCells = new int[rows, cols];

            for (var row = 0; row < rows; ++row)
            {
                for (var column = 0; column < cols; ++column)
                {
                    var liveNeighbourCount = 0;
                    for (var i = Math.Max(0, row - 1); i < Math.Min(rows, row + 2); ++i)
                    {
                        for (var j = Math.Max(0, column - 1); j < Math.Min(cols, column + 2); ++j)
                        {
                            if (i == row && j == column)
                                continue;

                            liveNeighbourCount += extendedCells[i, j];
                        }
                    }

                    if (extendedCells[row, column] == 1)
                    {
                        if (liveNeighbourCount is < 2 or > 3)
                            newCells[row, column] = 0;
                        else
                            newCells[row, column] = 1;
                    }
                    else
                    {
                        if (liveNeighbourCount == 3)
                            newCells[row, column] = 1;
                    }
                }
            }

            result = newCells;
        }

        return TrimCells(result);
    }

    public static int[,] ExtrudeCells(int[,] cells)
    {
        var rows = cells.GetLength(0);
        var cols = cells.GetLength(1);
        var extrudedCells = new int[rows + 2, cols + 2];

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                extrudedCells[i + 1, j + 1] = cells[i, j];
            }
        }

        return extrudedCells;
    }

    public static int[,] TrimCells(int[,] cells)
    {
        int top = 0, bottom = cells.GetLength(0) - 1;
        int left = 0, right = cells.GetLength(1) - 1;

        // Find the top boundary
        while (top <= bottom && IsRowEmpty(cells, top))
        {
            top++;
        }

        // Find the bottom boundary
        while (bottom >= top && IsRowEmpty(cells, bottom))
        {
            bottom--;
        }

        // Find the left boundary
        while (left <= right && IsColumnEmpty(cells, left))
        {
            left++;
        }

        // Find the right boundary
        while (right >= left && IsColumnEmpty(cells, right))
        {
            right--;
        }

        // Create the trimmed array
        var newRows = bottom - top + 1;
        var newCols = right - left + 1;
        var trimmedCells = new int[newRows, newCols];

        for (var i = 0; i < newRows; i++)
        {
            for (var j = 0; j < newCols; j++)
            {
                trimmedCells[i, j] = cells[top + i, left + j];
            }
        }

        return trimmedCells;
    }

    private static bool IsRowEmpty(int[,] cells, int row)
    {
        for (var j = 0; j < cells.GetLength(1); ++j)
        {
            if (cells[row, j] == 1) return false;
        }

        return true;
    }

    private static bool IsColumnEmpty(int[,] cells, int col)
    {
        for (var i = 0; i < cells.GetLength(0); ++i)
        {
            if (cells[i, col] == 1) return false;
        }

        return true;
    }

    public static string Htmlize(int[,] cells)
    {
        var result = new StringBuilder();
        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                result.Append(cells[i, j] == 1 ? "1" : "0");
            }

            result.Append('\n');
        }

        return result.ToString();
    }
}