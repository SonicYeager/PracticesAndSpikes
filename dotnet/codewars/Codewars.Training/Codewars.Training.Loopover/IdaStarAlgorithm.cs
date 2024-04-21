using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public static class IdaStarAlgorithm
{
    public static List<string>? Solve(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        var startNode = new BoardNode
        {
            Board = mixedUpBoard, Moves = new(), Parent = default,
        };
        var endNode = new BoardNode
        {
            Board = solvedBoard, Moves = new(), Parent = default,
        };

        var threshold = Heuristic(startNode.Board, endNode.Board);
        while (true)
        {
            var temp = Search(startNode, 0, threshold, endNode.Board);
            switch (temp.Item1)
            {
                case -1:
                    return temp.Item2.Moves.Select(static m => m.ToString()).ToList();
                case int.MaxValue / 1000000:
                    return null;
                default:
                    threshold = temp.Item1;
                    break;
            }
        }
    }

    private static (int, BoardNode?) Search(BoardNode node, int g, int threshold, char[][] endBoard)
    {
        var f = g + Heuristic(node.Board, endBoard);
        if (f > threshold) return (f, null);
        if (AreBoardsEqual(node.Board, endBoard)) return (-1, node);

        var min = int.MaxValue;
        foreach (var (heuristic, foundNode) in GetNeighbors(node).Select(neighbor => Search(neighbor, g + 1, threshold, endBoard)))
        {
            if (heuristic == -1) return (-1, foundNode);
            if (heuristic < min) min = heuristic;
        }

        return (min, null);
    }

    private static int Heuristic(char[][] currentBoard, char[][] endBoard)
    {
        var totalDistance = 0;

        for (var i = 0; i < currentBoard.Length; i++)
        {
            for (var j = 0; j < currentBoard[i].Length; j++)
            {
                var currentTile = currentBoard[i][j];
                var (goalRow, goalCol) = FindPosition(endBoard, currentTile);
                var distance = Math.Abs(i - goalRow) + Math.Abs(j - goalCol);
                totalDistance += distance;
            }
        }

        return totalDistance;
    }

    private static (int, int) FindPosition(char[][] board, char tile)
    {
        for (var i = 0; i < board.Length; i++)
        {
            for (var j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == tile) return (i, j);
            }
        }

        throw new Exception($"Tile {tile} not found in board");
    }

    private static string BoardToString(char[][] neighborBoard)
    {
        return string.Join("", neighborBoard.Select(static row => new string(row)));
    }

    private static bool AreBoardsEqual(char[][] currentNodeBoard, char[][] endNodeBoard)
    {
        var currentBoardString = BoardToString(currentNodeBoard);
        var endBoardString = BoardToString(endNodeBoard);

        return currentBoardString == endBoardString;
    }

    private static List<BoardNode> GetNeighbors(BoardNode boardNode)
    {
        var neighbors = new List<BoardNode>();

        for (var i = 0; i < boardNode.Board.Length; i++)
        {
            // Shift row i to the left and to the right
            var movedLeft = new Move(MoveDirection.Left, i);
            var leftShiftedBoard = movedLeft.Apply(boardNode.Board);
            var movedRight = new Move(MoveDirection.Right, i);
            var rightShiftedBoard = movedRight.Apply(boardNode.Board);

            neighbors.Add(new()
            {
                Board = leftShiftedBoard,
                Moves = new(boardNode.Moves)
                {
                    new($"L{i}"),
                },
                Parent = boardNode,
            });

            neighbors.Add(new()
            {
                Board = rightShiftedBoard,
                Moves = new(boardNode.Moves)
                {
                    new($"R{i}"),
                },
                Parent = boardNode,
            });
        }

        for (var i = 0; i < boardNode.Board[0].Length; i++)
        {
            // Shift column i up and down
            var movedUp = new Move(MoveDirection.Up, i);
            var upShiftedBoard = movedUp.Apply(boardNode.Board);
            var movedDown = new Move(MoveDirection.Down, i);
            var downShiftedBoard = movedDown.Apply(boardNode.Board);

            neighbors.Add(new()
            {
                Board = upShiftedBoard,
                Moves = new(boardNode.Moves)
                {
                    new($"U{i}"),
                },
                Parent = boardNode,
            });

            neighbors.Add(new()
            {
                Board = downShiftedBoard,
                Moves = new(boardNode.Moves)
                {
                    new($"D{i}"),
                },
                Parent = boardNode,
            });
        }

        return neighbors;
    }
}