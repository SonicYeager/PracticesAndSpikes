using Codewars.Training.ProdOfFib;

namespace Codewars.Training;

[TestFixture]
public class ProdFibTests
{
    [Test]
    public void Test1()
    {
        var r = new ulong[]
        {
            55, 89, 1,
        };
        Assert.That(Kata.ProductFib(4895), Is.EqualTo(r));
    }

    [Test]
    public void Test2()
    {
        var r = new ulong[]
        {
            89, 144, 0,
        };
        Assert.That(Kata.ProductFib(5895), Is.EqualTo(r));
    }

    [Test]
    public void Test3()
    {
        var r = new ulong[]
        {
            6765, 10946, 1,
        };
        Assert.That(Kata.ProductFib(74049690), Is.EqualTo(r));
    }

    [Test]
    public void Test4()
    {
        var r = new ulong[]
        {
            10946, 17711, 0,
        };
        Assert.That(Kata.ProductFib(84049690), Is.EqualTo(r));
    }

    [Test]
    public void Test5()
    {
        var r = new ulong[]
        {
            10946, 17711, 1,
        };
        Assert.That(Kata.ProductFib(193864606), Is.EqualTo(r));
    }

    [Test]
    public void Test6()
    {
        var r = new ulong[]
        {
            610, 987, 0,
        };
        Assert.That(Kata.ProductFib(447577), Is.EqualTo(r));
    }

    [Test]
    public void Test7()
    {
        var r = new ulong[]
        {
            610, 987, 1,
        };
        Assert.That(Kata.ProductFib(602070), Is.EqualTo(r));
    }

    [Test]
    public void Test8()
    {
        var r = new ulong[]
        {
            832040, 1346269, 0,
        };
        Assert.That(Kata.ProductFib(602070602070), Is.EqualTo(r));
    }

    [Test]
    public void Test9()
    {
        var r = new ulong[]
        {
            832040, 1346269, 1,
        };
        Assert.That(Kata.ProductFib(1120149658760), Is.EqualTo(r));
    }

    [Test]
    public void Test10()
    {
        var r = new ulong[]
        {
            1836311903, 2971215073, 0,
        };
        Assert.That(Kata.ProductFib(2563195080744681828), Is.EqualTo(r));
    }

    [Test]
    public void Test11()
    {
        var r = new ulong[]
        {
            1346269, 2178309, 1,
        };
        Assert.That(Kata.ProductFib(2932589879121), Is.EqualTo(r));
    }

    [Test]
    public void Test12()
    {
        var r = new ulong[]
        {
            1, 1, 1,
        };
        Assert.That(Kata.ProductFib(1), Is.EqualTo(r));
    }

    [Test]
    public void Test13()
    {
        var r = new ulong[]
        {
            2971215073, 4807526976, 0,
        };
        Assert.That(Kata.ProductFib(5456077604922913920), Is.EqualTo(r));
    }

    [Test]
    public void RandomTests()
    {
        ulong[] someFibs =
        [
            55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229,
            832040, 1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986,
        ];

        var rnd = new Random();
        for (var k = 0; k < 15; k++)
        {
            var rn = rnd.Next(0, 28);
            var f1 = someFibs[rn];
            var f2 = someFibs[rn + 1];
            var p = f1 * f2;
            var r = new[]
            {
                f1, f2, 1UL,
            };
            Assert.That(Kata.ProductFib(p), Is.EqualTo(r));
        }
    }
}