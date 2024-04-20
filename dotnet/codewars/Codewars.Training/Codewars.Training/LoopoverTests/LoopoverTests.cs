using Codewars.Training.Loopover;

namespace Codewars.Training.LoopoverTests;

[TestFixture]
public sealed class LoopoverTests
{
    //helper method that checks whether the moves lead to the solved board
    private static void CheckMoves(char[][] mixedUpBoard, char[][] solvedBoard, IEnumerable<Move> moves)
    {
        //apply all moves to the mixed up board
        var currentBoard = moves.Aggregate(mixedUpBoard, static (current, move) => move.Apply(current));

        Assert.That(solvedBoard, Is.EqualTo(currentBoard));
    }

    //tests for 2x2 ... 9x9 boards and some like 5x7 etc.
    [Test]
    public void Test2X2()
    {
        var mixedUpBoard = new[]
        {
            [
                'a', 'b',
            ],
            new[]
            {
                'c', 'd',
            },
        };
        var solvedBoard = new[]
        {
            [
                'a', 'b',
            ],
            new[]
            {
                'c', 'd',
            },
        };

        var moves = Kata.Solve(mixedUpBoard, solvedBoard).Select(m => new Move(m));
        CheckMoves(mixedUpBoard, solvedBoard, moves);
    }
}