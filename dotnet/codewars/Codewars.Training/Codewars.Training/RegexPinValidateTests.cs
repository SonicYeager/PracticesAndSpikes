using Codewars.Training.RegexPinValidate;

namespace Codewars.Training;

[TestFixture]
public class RegexPinValidateTests
{
    [Test]
    [Description("ValidatePin should return false for pins with length other than 4 or 6")]
    public void LengthTest()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.ValidatePin("1"), Is.EqualTo(false), "Wrong output for \"1\"");
            Assert.That(Kata.ValidatePin("12"), Is.EqualTo(false), "Wrong output for \"12\"");
            Assert.That(Kata.ValidatePin("123"), Is.EqualTo(false), "Wrong output for \"123\"");
            Assert.That(Kata.ValidatePin("12345"), Is.EqualTo(false), "Wrong output for \"12345\"");
            Assert.That(Kata.ValidatePin("1234567"), Is.EqualTo(false), "Wrong output for \"1234567\"");
            Assert.That(Kata.ValidatePin("-1234"), Is.EqualTo(false), "Wrong output for \"-1234\"");
            Assert.That(Kata.ValidatePin("1.234"), Is.EqualTo(false), "Wrong output for \"1.234\"");
            Assert.That(Kata.ValidatePin("-1.234"), Is.EqualTo(false), "Wrong output for \"-1.234\"");
            Assert.That(Kata.ValidatePin("00000000"), Is.EqualTo(false), "Wrong output for \"00000000\"");
        });
    }

    [Test]
    [Description("ValidatePin should return false for pins which contain characters other than digits")]
    public void NonDigitTest()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.ValidatePin("a234"), Is.EqualTo(false), "Wrong output for \"a234\"");
            Assert.That(Kata.ValidatePin(".234"), Is.EqualTo(false), "Wrong output for \".234\"");
        });
    }

    [Test]
    [Description("ValidatePin should return true for valid pins")]
    public void ValidTest()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.ValidatePin("1234"), Is.EqualTo(true), "Wrong output for \"1234\"");
            Assert.That(Kata.ValidatePin("0000"), Is.EqualTo(true), "Wrong output for \"0000\"");
            Assert.That(Kata.ValidatePin("1111"), Is.EqualTo(true), "Wrong output for \"1111\"");
            Assert.That(Kata.ValidatePin("123456"), Is.EqualTo(true), "Wrong output for \"123456\"");
            Assert.That(Kata.ValidatePin("098765"), Is.EqualTo(true), "Wrong output for \"098765\"");
            Assert.That(Kata.ValidatePin("000000"), Is.EqualTo(true), "Wrong output for \"000000\"");
            Assert.That(Kata.ValidatePin("090909"), Is.EqualTo(true), "Wrong output for \"090909\"");
        });
    }
}