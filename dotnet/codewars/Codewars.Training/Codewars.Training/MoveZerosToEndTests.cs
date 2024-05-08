using Codewars.Training.MoveZerosToEnd;

namespace Codewars.Training;

[TestFixture]
public class MoveZerosToEndTest
{
    private static readonly int[] Expected = new[]
    {
        1, 2, 1, 1, 3, 1, 0, 0, 0, 0,
    };

    [Test]
    public void Test()
    {
        Assert.That(Kata.MoveZeroes([
            1, 2, 0, 1, 0, 1, 0, 3, 0, 1,
        ]), Is.EqualTo(Expected));
    }
}