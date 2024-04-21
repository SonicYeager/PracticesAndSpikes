using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public sealed class IddfsAlgorithm
{
    private readonly char[][] _goalBoard;

    public IddfsAlgorithm(char[][] goalBoard)
    {
        _goalBoard = goalBoard;
    }

    public List<string>? Solve(char[][] startBoard)
    {
        var startNode = new BoardNode
        {
            Board = startBoard, Moves = new(), Parent = default,
        };

        for (int depth = 0; depth < int.MaxValue; depth++)
        {
            var result = Dls(startNode, depth);
            if (result != null) return result.Moves.Select(static m => m.ToString()).ToList();
        }

        return null;
    }

    private BoardNode? Dls(BoardNode node, int depth)
    {
        if (depth == 0 && AreBoardsEqual(node.Board, _goalBoard)) return node;

        if (depth > 0)
            foreach (var neighbor in GetNeighbors(node))
            {
                var result = Dls(neighbor, depth - 1);
                if (result != null) return result;
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