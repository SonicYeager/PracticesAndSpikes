using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public static class Kata
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

        var queue = new Queue<BoardNode>();
        var visited = new HashSet<string>();

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (AreBoardsEqual(currentNode.Board, endNode.Board)) return currentNode.Moves.Select(static m => m.ToString()).ToList();

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                var boardString = BoardToString(neighbor.Board);

                if (visited.Add(boardString)) queue.Enqueue(neighbor);
            }
        }

        return null;
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