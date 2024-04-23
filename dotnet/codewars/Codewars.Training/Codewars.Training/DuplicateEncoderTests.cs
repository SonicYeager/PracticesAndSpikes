using Codewars.Training.DuplicateEncoder;

namespace Codewars.Training;

[TestFixture]
public class DuplicateEncoderTests
{
    [Test]
    public void DuplicateEncode()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.DuplicateEncode("din"), Is.EqualTo("((("));
            Assert.That(Kata.DuplicateEncode("recede"), Is.EqualTo("()()()"));
            Assert.That(Kata.DuplicateEncode("Success"), Is.EqualTo(")())())"), "should ignore case");
            Assert.That(Kata.DuplicateEncode("(( @"), Is.EqualTo("))(("));
        });
    }
}