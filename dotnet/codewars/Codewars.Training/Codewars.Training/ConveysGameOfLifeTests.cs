using Codewars.Training.ConveysGameOfLife;

namespace Codewars.Training;

[TestFixture]
public sealed class ConveysGameOfLifeTests
{
    [Test]
    public void TestGlider()
    {
        DoTest(new[,]
        {
            {
                1, 0, 0,
            },
            {
                0, 1, 1,
            },
            {
                1, 1, 0,
            },
        }, 1, new[,]
        {
            {
                0, 1, 0,
            },
            {
                0, 0, 1,
            },
            {
                1, 1, 1,
            },
        });
    }

    [Test]
    public void TestTwoGliders()
    {
        DoTest(new[,]
        {
            {
                1, 1, 1, 0, 0, 0, 1, 0,
            },
            {
                1, 0, 0, 0, 0, 0, 0, 1,
            },
            {
                0, 1, 0, 0, 0, 1, 1, 1,
            },
        }, 16, new[,]
        {
            {
                1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
            },
        });
    }

    private static void DoTest(int[,] start, int generations, int[,] expected)
    {
        var inputRepr = Kata.Htmlize(start);
        var expectedRepr = Kata.Htmlize(expected);
        var actual = Kata.GetGeneration(start, generations);
        var actualRepr = Kata.Htmlize(actual);

        var message =
                "for cells:\n" + inputRepr + "after " + generations + " generations, " +
                "expected :\n" + expectedRepr + "\nbut got:\n" + actualRepr
            ;
        Assert.That(actual, Is.EqualTo(expected), message);
    }
}