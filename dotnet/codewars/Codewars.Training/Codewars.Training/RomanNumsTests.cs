using Codewars.Training.RomanNums;

namespace Codewars.Training;

[TestFixture]
public class RomanNumsTests
{
    [TestCase(1, "I")]
    [TestCase(2, "II")]
    [TestCase(3, "III")]
    [TestCase(4, "IV")]
    [TestCase(5, "V")]
    [TestCase(6, "VI")]
    [TestCase(7, "VII")]
    [TestCase(8, "VIII")]
    [TestCase(9, "IX")]
    public void Roman_SingleDigits(int input, string expected)
    {
        var actual = Kata.ToRoman(input);
        var actualBack = Kata.FromRoman(expected);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actualBack, Is.EqualTo(input));
        });
    }

    [TestCase(10, "X")]
    [TestCase(11, "XI")]
    [TestCase(14, "XIV")]
    [TestCase(15, "XV")]
    [TestCase(16, "XVI")]
    [TestCase(19, "XIX")]
    [TestCase(40, "XL")]
    [TestCase(90, "XC")]
    public void Roman_DoubleDigits(int input, string expected)
    {
        var actual = Kata.ToRoman(input);
        var actualBack = Kata.FromRoman(expected);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actualBack, Is.EqualTo(input));
        });
    }

    [TestCase(100, "C")]
    [TestCase(400, "CD")]
    [TestCase(500, "D")]
    [TestCase(900, "CM")]
    public void Roman_TripleDigits(int input, string expected)
    {
        var actual = Kata.ToRoman(input);
        var actualBack = Kata.FromRoman(expected);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actualBack, Is.EqualTo(input));
        });
    }

    [TestCase(1000, "M")]
    [TestCase(2000, "MM")]
    [TestCase(3000, "MMM")]
    [TestCase(1666, "MDCLXVI")]
    [TestCase(1990, "MCMXC")]
    public void Roman_QuadrupleDigits(int input, string expected)
    {
        var actual = Kata.ToRoman(input);
        var actualBack = Kata.FromRoman(expected);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actualBack, Is.EqualTo(input));
        });
    }
}