using Codewars.Training.HighestAndLowest;

namespace Codewars.Training;

[TestFixture]
public sealed class HighestAndLowestTests
{
    [Test]
    public void HighAndLow_WithNegativValuesOnly_AreCorrectlyInterpretedAsSmallerWithGreaterNegativity()
    {
        Assert.That(Kata.HighAndLow("-69 -4"), Is.EqualTo("-4 -69"));
    }

    [Test]
    public void HighAndLow_WithNegativValues_AreCorrectlyInterpretedAsSmaller()
    {
        Assert.That(Kata.HighAndLow("8 3 -5 42 -1 0 0 -9 4 7 4 -4"), Is.EqualTo("42 -9"));
    }

    [Test]
    public void HighAndLow_WithDuplicateValues_IgnoresDuplicates()
    {
        Assert.That(Kata.HighAndLow("1 3 3"), Is.EqualTo("3 1"));
    }

    [Test]
    public void HighAndLow_WithPositiveValues_Works()
    {
        Assert.That(Kata.HighAndLow("1 2 3"), Is.EqualTo("3 1"));
    }

    [Test]
    public void HighAndLow_WithSingleValue_ReturnsOnlySingleValueAsMinAndMax()
    {
        Assert.That(Kata.HighAndLow("1"), Is.EqualTo("1 1"));
    }
}