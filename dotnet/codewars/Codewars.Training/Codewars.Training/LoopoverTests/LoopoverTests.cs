using Codewars.Training.Loopover;

namespace Codewars.Training.LoopoverTests;

[TestFixture]
public sealed class LoopoverTests
{
    private static void CheckMoves(char[][] mixedUpBoard, char[][] solvedBoard, IEnumerable<Move> moves)
    {
        //apply all moves to the mixed up board
        var currentBoard = moves.Aggregate(mixedUpBoard, static (current, move) => move.Apply(current));

        Assert.That(solvedBoard, Is.EqualTo(currentBoard));
    }

    [TestCaseSource(nameof(BoardTestCases))]
    public void Solve(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        var moves = Kata.Solve(mixedUpBoard, solvedBoard)?.Select(static m => new Move(m)).ToList();
        Assert.That(moves, Is.Not.Null);
        CheckMoves(mixedUpBoard, solvedBoard, moves);
    }

    public static IEnumerable<TestCaseData> BoardTestCases
    {
        get
        {
            yield return new TestCaseData(
                new char[][]
                {
                    ['d', 'c',], ['b', 'a',],
                },
                new char[][]
                {
                    ['a', 'b',], ['c', 'd',],
                }
            ).SetName("Test2X2_WithAlreadySolvedBoard_ShouldReturnMoves");

            yield return new TestCaseData(
                new char[][]
                {
                    ['i', 'h', 'g'], ['f', 'e', 'd'], ['c', 'b', 'a'],
                },
                new char[][]
                {
                    ['a', 'b', 'c'], ['d', 'e', 'f'], ['g', 'h', 'i'],
                }
            ).SetName("Test3X3_WithAlreadySolvedBoard_ShouldReturnMoves");

            yield return new TestCaseData(
                new char[][]
                {
                    ['p', 'o', 'n', 'm'], ['l', 'k', 'j', 'i'], ['h', 'g', 'f', 'e'], ['d', 'c', 'b', 'a']
                },
                new char[][]
                {
                    ['a', 'b', 'c', 'd'], ['e', 'f', 'g', 'h'], ['i', 'j', 'k', 'l'], ['m', 'n', 'o', 'p']
                }
            ).SetName("Test4X4_WithAlreadySolvedBoard_ShouldReturnMoves");

            yield return new TestCaseData(
                new char[][]
                {
                    ['y', 'x', 'w', 'v', 'u'],
                    ['t', 's', 'r', 'q', 'p'],
                    ['o', 'n', 'm', 'l', 'k'],
                    ['j', 'i', 'h', 'g', 'f'],
                    ['e', 'd', 'c', 'b', 'a'],
                },
                new char[][]
                {
                    ['a', 'b', 'c', 'd', 'e'],
                    ['f', 'g', 'h', 'i', 'j'],
                    ['k', 'l', 'm', 'n', 'o'],
                    ['p', 'q', 'r', 's', 't'],
                    ['u', 'v', 'w', 'x', 'y'],
                }
            ).SetName("Test5X5_WithAlreadySolvedBoard_ShouldReturnMoves");

            yield return new TestCaseData(
                new char[][]
                {
                    [
                        'b', 'a', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                    ],
                    [
                        'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                    ],
                    [
                        's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1',
                    ],
                    [
                        '2', '3', '4', '5', '6', '7', '8', '9', '0',
                    ],
                    [
                        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
                    ],
                    [
                        'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
                    ],
                    [
                        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '!',
                    ],
                    [
                        '@', '#', '$', '%', '^', '&', '*', '(', ')',
                    ],
                    [
                        '-', '=', '+', '[', ']', '{', '}', ';', ':',
                    ],
                },
                new char[][]
                {
                    [
                        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                    ],
                    [
                        'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                    ],
                    [
                        's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1',
                    ],
                    [
                        '2', '3', '4', '5', '6', '7', '8', '9', '0',
                    ],
                    [
                        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
                    ],
                    [
                        'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
                    ],
                    [
                        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '!',
                    ],
                    [
                        '@', '#', '$', '%', '^', '&', '*', '(', ')',
                    ],
                    [
                        '-', '=', '+', '[', ']', '{', '}', ';', ':',
                    ],
                }
            ).SetName("Test9X9_WithAlreadySolvedBoard_ShouldReturnMoves");
        }
    }

    [TestCaseSource(nameof(UnsolvableBoards))]
    public void Solve_ShouldReturnNull(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        var moves = Kata.Solve(mixedUpBoard, solvedBoard)?.Select(static m => new Move(m)).ToList();
        Assert.That(moves, Is.Null);
    }
    public static IEnumerable<TestCaseData> UnsolvableBoards
    {
        get
        {
            yield return new TestCaseData(
                new char[][]
                {
                    [
                        'a', 'b', 'c',
                    ],
                    [
                        'd', 'e', 'f',
                    ],
                    [
                        'g', 'h', 'i',
                    ],
                },
                new char[][]
                {
                    [
                        'b', 'a', 'c',
                    ],
                    [
                        'd', 'e', 'f',
                    ],
                    [
                        'g', 'h', 'i',
                    ],
                }
            ).SetName("Test3X3_WithUnsolvableBoard_ShouldReturnNull");

            yield return new TestCaseData(
                new char[][]
                {
                    [
                        'a', 'b', 'c',
                    ],
                    [
                        'd', 'e', 'f',
                    ],
                    [
                        'g', 'h', 'i',
                    ],
                },
                new char[][]
                {
                    [
                        'c', 'b', 'a',
                    ],
                    [
                        'd', 'e', 'f',
                    ],
                    [
                        'g', 'h', 'i',
                    ],
                }
            ).SetName("Test3X3_WithAnotherUnsolvableBoard_ShouldReturnNull");
        }
    }

}