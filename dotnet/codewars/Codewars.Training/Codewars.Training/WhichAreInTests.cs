using Codewars.Training.WhichAreIn;

namespace Codewars.Training;

[TestFixture]
public class WhichAreInTests
{

    [Test]
    public void InArray()
    {
        var a1 = new[]
        {
            "arp", "live", "strong",
        };
        var a2 = new[]
        {
            "lively", "alive", "harp", "sharp", "armstrong",
        };
        var r = new[]
        {
            "arp", "live", "strong",
        };
        Assert.That(Kata.InArray(a1, a2), Is.EqualTo(r));
    }

    [Test]
    public void InArray_IsSortedLexicographically()
    {
        var a1 = new[]
        {
            "strong", "live", "arp",
        };
        var a2 = new[]
        {
            "lively", "alive", "harp", "sharp", "armstrong",
        };
        var r = new[]
        {
            "arp", "live", "strong",
        };
        Assert.That(Kata.InArray(a1, a2), Is.EqualTo(r));
    }
}