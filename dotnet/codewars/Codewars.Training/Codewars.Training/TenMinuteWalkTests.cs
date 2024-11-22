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
            Assert.That(Kata.IsValidWalk([
                    "n", "s", "n", "s", "n", "s", "n", "s", "n", "s",
                ]),
                Is.True,
                "should return true");
            Assert.That(Kata.IsValidWalk([
                    "w", "e", "w", "e", "w", "e", "w", "e", "w", "e", "w", "e",
                ]),
                Is.False,
                "should return false");
            Assert.That(Kata.IsValidWalk([
                    "w",
                ]),
                Is.False,
                "should return false");
            Assert.That(Kata.IsValidWalk([
                    "n", "n", "n", "s", "n", "s", "n", "s", "n", "s",
                ]),
                Is.False,
                "should return false");
        });
    }
}