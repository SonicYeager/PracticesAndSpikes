using Codewars.Training.Loopover;

namespace Codewars.Training.LoopoverTests;

[TestFixture]
public class MoveTests
{
    [Test]
    public void Move_CtorDirectInitOfProps_ShouldSetProps()
    {
        const MoveDirection direction = MoveDirection.Up;
        const int index = 1;

        var move = new Move(direction, index);

        Assert.Multiple(() =>
        {
            Assert.That(move.Direction, Is.EqualTo(direction));
            Assert.That(move.Index, Is.EqualTo(index));
        });
    }

    [Test]
    public void Move_CtorString_ShouldSetProps()
    {
        const string moveStr = "U1";
        const MoveDirection direction = MoveDirection.Up;
        const int index = 1;

        var move = new Move(moveStr);

        Assert.Multiple(() =>
        {
            Assert.That(move.Direction, Is.EqualTo(direction));
            Assert.That(move.Index, Is.EqualTo(index));
        });
    }

    [TestCase("U1")]
    [TestCase("D2")]
    [TestCase("L3")]
    [TestCase("R44")]
    public void ToString_ShouldReturnCorrectString(string moveStr)
    {
        var move = new Move(moveStr);

        Assert.That(move.ToString(), Is.EqualTo(moveStr));
    }

    [Test]
    public void Apply_ShouldReturnShiftedColumnUp()
    {
        var board = new[]
        {
            [
                'a', 'b', 'c',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var expected = new[]
        {
            [
                'd', 'b', 'c',
            ],
            [
                'g', 'e', 'f',
            ],
            new[]
            {
                'a', 'h', 'i',
            },
        };
        var move = new Move(MoveDirection.Up, 0);

        var actual = move.Apply(board);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Apply_ShouldReturnShiftedColumnDown()
    {
        var board = new[]
        {
            [
                'a', 'b', 'c',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var expected = new[]
        {
            [
                'g', 'b', 'c',
            ],
            [
                'a', 'e', 'f',
            ],
            new[]
            {
                'd', 'h', 'i',
            },
        };
        var move = new Move(MoveDirection.Down, 0);

        var actual = move.Apply(board);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Apply_ShouldReturnShiftedRowLeft()
    {
        var board = new[]
        {
            [
                'a', 'b', 'c',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var expected = new[]
        {
            [
                'b', 'c', 'a',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var move = new Move(MoveDirection.Left, 0);

        var actual = move.Apply(board);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Apply_ShouldReturnShiftedRowRight()
    {
        var board = new[]
        {
            [
                'a', 'b', 'c',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var expected = new[]
        {
            [
                'c', 'a', 'b',
            ],
            [
                'd', 'e', 'f',
            ],
            new[]
            {
                'g', 'h', 'i',
            },
        };
        var move = new Move(MoveDirection.Right, 0);

        var actual = move.Apply(board);

        Assert.That(actual, Is.EqualTo(expected));
    }
}