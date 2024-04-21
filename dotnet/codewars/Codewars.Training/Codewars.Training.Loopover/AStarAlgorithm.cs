using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public sealed class AStarAlgorithm
{
    private readonly char[][] _goalBoard;
    private readonly Dictionary<string, BoardNode> _visitedNodes;
    private readonly PriorityQueue<BoardNode, int> _frontier;
    private readonly Dictionary<char, (int, int)> _goalPositions;

    public AStarAlgorithm(char[][] goalBoard)
    {
        _goalBoard = goalBoard;
        _visitedNodes = new();
        _frontier = new();
        _goalPositions = new();

        for (var i = 0; i < goalBoard.Length; i++)
        {
            for (var j = 0; j < goalBoard[i].Length; j++)
            {
                _goalPositions[goalBoard[i][j]] = (i, j);
            }
        }
    }

    public List<string>? Solve(char[][] startBoard)
    {
        var startNode = new BoardNode
        {
            Board = startBoard, Moves = new(), Parent = default,
        };

        _frontier.Enqueue(startNode, 0);

        while (_frontier.Count > 0)
        {
            var currentNode = _frontier.Dequeue();

            if (AreBoardsEqual(currentNode.Board, _goalBoard))
            {
                return currentNode.Moves.Select(static m => m.ToString()).ToList();
            }

            var currentBoardString = BoardToString(currentNode.Board);
            if (_visitedNodes.ContainsKey(currentBoardString) && _visitedNodes[currentBoardString].Moves.Count <= currentNode.Moves.Count)
            {
                continue;
            }

            _visitedNodes[currentBoardString] = currentNode;

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                var newCost = currentNode.Moves.Count + 1;
                var neighborBoardString = BoardToString(neighbor.Board);

                if (!_visitedNodes.ContainsKey(neighborBoardString) || _visitedNodes[neighborBoardString].Moves.Count > newCost)
                {
                    var priority = newCost + Heuristic(neighbor.Board, _goalBoard);
                    _frontier.Enqueue(neighbor, priority);
                }
            }
        }

        return null;
    }

    private int Heuristic(char[][] currentBoard, char[][] endBoard)
    {
        var totalDistance = 0;

        for (var i = 0; i < currentBoard.Length; i++)
        {
            for (var j = 0; j < currentBoard[i].Length; j++)
            {
                var currentTile = currentBoard[i][j];
                var (goalRow, goalCol) = _goalPositions[currentTile]; // Use the lookup table here
                var distance = Math.Abs(i - goalRow) + Math.Abs(j - goalCol);
                totalDistance += distance;
            }
        }

        return totalDistance;
    }

    private static string BoardToString(char[][] neighborBoard)
    {
        return string.Join("", neighborBoard.Select(static row => new string(row)));
    }

    private static bool AreBoardsEqual(char[][] board1, char[][] board2)
    {
        if (board1.Length != board2.Length || board1[0].Length != board2[0].Length) return false;

        for (var i = 0; i < board1.Length; i++)
        {
            for (var j = 0; j < board1[i].Length; j++)
            {
                if (board1[i][j] != board2[i][j]) return false;
            }
        }

        return true;
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

            // Avoid moving a row that was just moved in the opposite direction
            if (boardNode.Moves.Count == 0 || boardNode.Moves.Last().Direction != MoveDirection.Right || boardNode.Moves.Last().Index != i)
            {
                neighbors.Add(new()
                {
                    Board = leftShiftedBoard,
                    Moves = new(boardNode.Moves)
                    {
                        new($"L{i}"),
                    },
                    Parent = boardNode,
                });
            }

            if (boardNode.Moves.Count == 0 || boardNode.Moves.Last().Direction != MoveDirection.Left || boardNode.Moves.Last().Index != i)
            {
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
        }

        for (var i = 0; i < boardNode.Board[0].Length; i++)
        {
            // Shift column i up and down
            var movedUp = new Move(MoveDirection.Up, i);
            var upShiftedBoard = movedUp.Apply(boardNode.Board);
            var movedDown = new Move(MoveDirection.Down, i);
            var downShiftedBoard = movedDown.Apply(boardNode.Board);

            // Avoid moving a column that was just moved in the opposite direction
            if (boardNode.Moves.Count == 0 || boardNode.Moves.Last().Direction != MoveDirection.Down || boardNode.Moves.Last().Index != i)
            {
                neighbors.Add(new()
                {
                    Board = upShiftedBoard,
                    Moves = new(boardNode.Moves)
                    {
                        new($"U{i}"),
                    },
                    Parent = boardNode,
                });
            }

            if (boardNode.Moves.Count == 0 || boardNode.Moves.Last().Direction != MoveDirection.Up || boardNode.Moves.Last().Index != i)
            {
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
        }

        return neighbors;
    }
}