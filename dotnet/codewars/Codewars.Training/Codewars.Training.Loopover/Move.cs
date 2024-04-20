using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.Loopover;

public sealed class Move
{
    public Move(MoveDirection direction, int index)
    {
        Direction = direction;
        Index = index;
    }

    public Move(string move)
    {
        Direction = move[0] switch
        {
            'U' => MoveDirection.Up,
            'D' => MoveDirection.Down,
            'L' => MoveDirection.Left,
            'R' => MoveDirection.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(move), move, null),
        };
        Index = int.Parse(move[1..]);
    }

    public MoveDirection Direction { get; }
    public int Index { get; }

    public char[][] Apply(char[][] currentBoard)
    {
        return Direction switch
        {
            MoveDirection.Up => ShiftColumnUp(currentBoard, Index),
            MoveDirection.Down => ShiftColumnDown(currentBoard, Index),
            MoveDirection.Left => ShiftRowLeft(currentBoard, Index),
            MoveDirection.Right => ShiftRowRight(currentBoard, Index),
            _ => throw new ArgumentOutOfRangeException(nameof(Direction), "Unknown Direction"),
        };
    }

    public string ToString(MoveDirection direction, int index)
    {
        return direction switch
        {
            MoveDirection.Up => Up(index),
            MoveDirection.Down => Down(index),
            MoveDirection.Left => Left(index),
            MoveDirection.Right => Right(index),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }

    private static string Up(int index) => $"U{index}";
    private static string Down(int index) => $"D{index}";
    private static string Left(int index) => $"L{index}";
    private static string Right(int index) => $"R{index}";

    private static char[][] ShiftRowLeft(char[][] board, int rowIndex)
    {
        var newBoard = (char[][])board.Clone();
        var row = new List<char>(newBoard[rowIndex]);
        var first = row[0];
        row.RemoveAt(0);
        row.Add(first);
        newBoard[rowIndex] = row.ToArray();
        return newBoard;
    }

    private static char[][] ShiftRowRight(char[][] board, int rowIndex)
    {
        var newBoard = (char[][])board.Clone();
        var row = new List<char>(newBoard[rowIndex]);
        var last = row[^1];
        row.RemoveAt(row.Count - 1);
        row.Insert(0, last);
        newBoard[rowIndex] = row.ToArray();
        return newBoard;
    }

    private static char[][] ShiftColumnUp(char[][] board, int columnIndex)
    {
        var newBoard = (char[][])board.Clone();
        var column = board.Select(t => t[columnIndex]).ToList();

        var first = column[0];
        column.RemoveAt(0);
        column.Add(first);
        for (var i = 0; i < board.Length; i++)
        {
            newBoard[i][columnIndex] = column[i];
        }

        return newBoard;
    }

    private static char[][] ShiftColumnDown(char[][] board, int columnIndex)
    {
        var newBoard = (char[][])board.Clone();
        var column = board.Select(t => t[columnIndex]).ToList();

        var last = column[^1];
        column.RemoveAt(column.Count - 1);
        column.Insert(0, last);
        for (var i = 0; i < board.Length; i++)
        {
            newBoard[i][columnIndex] = column[i];
        }

        return newBoard;
    }
}