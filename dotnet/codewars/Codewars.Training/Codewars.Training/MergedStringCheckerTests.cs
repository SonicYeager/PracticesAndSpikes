using Codewars.Training.MergedStringChecker;

namespace Codewars.Training;

[TestFixture]
public sealed class MergedStringCheckerTests
{
    [Test]
    public void HappyPath1()
    {
        Assert.That(Kata.isMerge("codewars", "code", "wars"), Is.True);
    }

    [Test]
    public void HappyPath2()
    {
        Assert.That(Kata.isMerge("codewars", "cdwr", "oeas"), Is.True);
    }

    [Test]
    public void SadPath1()
    {
        Assert.That(Kata.isMerge("codewars", "cod", "wars"), Is.False);
    }

    [Test]
    public void SadPath2()
    {
        Assert.That(Kata.isMerge("codewars", "codd", "wars"), Is.False);
    }

    [Test]
    public void HappyPath4()
    {
        Assert.That(Kata.isMerge("Can we merge it? Yes, we can!", "n ee tYw n!", "Cawe mrgi? es, eca"), Is.True);
    }
}