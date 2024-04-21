using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public static class Kata
{
    public static List<string> Solve(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        return null;
    }


    public class BidirectionalSearch
    {
        private char[][] _startBoard;
        private char[][] _goalBoard;
        private Dictionary<string, BoardNode> _forwardVisited;
        private Dictionary<string, BoardNode> _backwardVisited;
        private PriorityQueue<BoardNode, int> _forwardFrontier;
        private PriorityQueue<BoardNode, int> _backwardFrontier;

        private readonly Dictionary<char, (int, int)> _goalPositions;
        private readonly Dictionary<char, (int, int)> _goalEndPositions;

        public BidirectionalSearch(char[][] startBoard, char[][] goalBoard)
        {
            _startBoard = startBoard;
            _goalBoard = goalBoard;
            _forwardVisited = new();
            _backwardVisited = new();
            _forwardFrontier = new();
            _backwardFrontier = new();

            _goalPositions = new();

            for (var i = 0; i < _goalBoard.Length; i++)
            {
                for (var j = 0; j < _goalBoard[i].Length; j++)
                {
                    _goalPositions[_goalBoard[i][j]] = (i, j);
                }
            }

            _goalEndPositions = new();

            for (var i = 0; i < _startBoard.Length; i++)
            {
                for (var j = 0; j < _startBoard[i].Length; j++)
                {
                    _goalEndPositions[_startBoard[i][j]] = (i, j);
                }
            }
        }

        public List<string> Solve()
        {
            // TODO: Implement the bidirectional A* search algorithm
            return null;
        }

        private List<BoardNode> GetNeighbors(BoardNode boardNode, bool isForwardSearch)
        {
            var neighbors = new List<BoardNode>();

            for (var i = 0; i < boardNode.Board.Length; i++)
            {
                // Shift row i to the left and to the right
                var leftShiftedBoard = ShiftRow(boardNode.Board, i, -1);
                var rightShiftedBoard = ShiftRow(boardNode.Board, i, 1);

                // Avoid moving a row that was just moved in the opposite direction
                if (boardNode.Moves.Count == 0 || boardNode.Moves.Last() != $"R{i}")
                {
                    neighbors.Add(new BoardNode
                    {
                        Board = leftShiftedBoard,
                        Moves = new List<string>(boardNode.Moves)
                        {
                            $"L{i}"
                        },
                        Parent = boardNode
                    });
                }

                if (boardNode.Moves.Count == 0 || boardNode.Moves.Last() != $"L{i}")
                {
                    neighbors.Add(new BoardNode
                    {
                        Board = rightShiftedBoard,
                        Moves = new List<string>(boardNode.Moves)
                        {
                            $"R{i}"
                        },
                        Parent = boardNode
                    });
                }
            }

            for (var i = 0; i < boardNode.Board[0].Length; i++)
            {
                // Shift column i up and down
                var upShiftedBoard = ShiftColumn(boardNode.Board, i, -1);
                var downShiftedBoard = ShiftColumn(boardNode.Board, i, 1);

                // Avoid moving a column that was just moved in the opposite direction
                if (boardNode.Moves.Count == 0 || boardNode.Moves.Last() != $"D{i}")
                {
                    neighbors.Add(new BoardNode
                    {
                        Board = upShiftedBoard,
                        Moves = new List<string>(boardNode.Moves)
                        {
                            $"U{i}"
                        },
                        Parent = boardNode
                    });
                }

                if (boardNode.Moves.Count == 0 || boardNode.Moves.Last() != $"U{i}")
                {
                    neighbors.Add(new BoardNode
                    {
                        Board = downShiftedBoard,
                        Moves = new List<string>(boardNode.Moves)
                        {
                            $"D{i}"
                        },
                        Parent = boardNode
                    });
                }
            }

            return neighbors;
        }

        private char[][] ShiftRow(char[][] board, int rowIndex, int shiftAmount)
        {
            var shiftedBoard = (char[][])board.Clone();
            var rowLength = board[0].Length;
            shiftAmount = ((shiftAmount % rowLength) + rowLength) % rowLength;
            var shiftedRow = new char[rowLength];

            for (var i = 0; i < rowLength; i++)
            {
                shiftedRow[(i + shiftAmount) % rowLength] = board[rowIndex][i];
            }

            shiftedBoard[rowIndex] = shiftedRow;
            return shiftedBoard;
        }

        private char[][] ShiftColumn(char[][] board, int columnIndex, int shiftAmount)
        {
            var shiftedBoard = (char[][])board.Clone();
            var columnLength = board.Length;
            shiftAmount = ((shiftAmount % columnLength) + columnLength) % columnLength;
            var shiftedColumn = new char[columnLength];

            for (var i = 0; i < columnLength; i++)
            {
                shiftedColumn[(i + shiftAmount) % columnLength] = board[i][columnIndex];
            }

            for (var i = 0; i < columnLength; i++)
            {
                shiftedBoard[i][columnIndex] = shiftedColumn[i];
            }

            return shiftedBoard;
        }

        private void Explore(PriorityQueue<BoardNode, int> frontier, Dictionary<string, BoardNode> visited, bool isForwardSearch)
        {
            // TODO: Explore from the current node, adding to the visited set and frontier
        }

        private int Heuristic(BoardNode node, bool isForwardSearch)
        {
            var currentBoard = node.Board;
            var endBoard = isForwardSearch ? _goalBoard : _startBoard;
            var totalDistance = 0;

            for (var i = 0; i < currentBoard.Length; i++)
            {
                for (var j = 0; j < currentBoard[i].Length; j++)
                {
                    var currentTile = currentBoard[i][j];
                    var (goalRow, goalCol) = isForwardSearch ? _goalPositions[currentTile] : _goalEndPositions[currentTile];
                    var distance = Math.Abs(i - goalRow) + Math.Abs(j - goalCol);
                    totalDistance += distance;
                }
            }

            return totalDistance;
        }
    }

    public class BoardNode
    {
        public char[][] Board { get; set; }
        public List<string> Moves { get; set; }
        public BoardNode Parent { get; set; }

        public BoardNode()
        {
            Moves = new List<string>();
        }
    }
}