using Codewars.Training.CountIpAddresses;

namespace Codewars.Training;

[TestFixture]
public sealed class CountIpAddressesTest
{
    [Test]
    public void SampleTest()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Kata.IpsBetween("10.0.0.0", "10.0.0.50"), Is.EqualTo(50));
            Assert.That(Kata.IpsBetween("20.0.0.10", "20.0.1.0"), Is.EqualTo(246));
            Assert.That(Kata.IpsBetween("0.0.0.0", "255.255.255.255"), Is.EqualTo((1L << 32) - 1L));
        });
    }
}