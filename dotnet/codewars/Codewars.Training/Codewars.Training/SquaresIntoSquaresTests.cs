using Codewars.Training.SquaresIntoSquares;

namespace Codewars.Training;

[TestFixture]
public sealed class SquaresIntoSquaresTests
{
    [Test]
    public void Test1()
    {
        const long n = 11;
        Assert.That(Kata.Decompose(n), Is.EqualTo("1 2 4 10"));
    }

    [Test]
    public void Test2()
    {
        const long n = 12;
        Assert.That(Kata.Decompose(n), Is.EqualTo("1 2 3 7 9"));
    }

    [Test]
    public void Test3()
    {
        const long n = 7100;
        Assert.That(Kata.Decompose(n), Is.EqualTo("2 3 5 119 7099")); //50410000
    }
}