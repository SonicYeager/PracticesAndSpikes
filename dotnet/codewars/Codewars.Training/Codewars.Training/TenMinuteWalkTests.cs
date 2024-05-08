using Codewars.Training.TenMinuteWalk;

namespace Codewars.Training;

[TestFixture]
public sealed class TenMinuteWalkTests
{
    [Test]
    public void SampleTest()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.IsValidWalk(new[]
                {
                    "n", "s", "n", "s", "n", "s", "n", "s", "n", "s",
                }),
                Is.True,
                "should return true");
            Assert.That(Kata.IsValidWalk(new[]
                {
                    "w", "e", "w", "e", "w", "e", "w", "e", "w", "e", "w", "e",
                }),
                Is.False,
                "should return false");
            Assert.That(Kata.IsValidWalk(new[]
                {
                    "w",
                }),
                Is.False,
                "should return false");
            Assert.That(Kata.IsValidWalk(new[]
                {
                    "n", "n", "n", "s", "n", "s", "n", "s", "n", "s",
                }),
                Is.False,
                "should return false");
        });
    }
}