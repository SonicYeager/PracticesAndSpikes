using AlgorithmTester.Logic;
using AlgorithmTester.Objects;
using FluentAssertions;
using NUnit.Framework;

namespace AlgorithmTester;

public class LogicTests
{
    [Test]
    public void Test()
    {
        // to perform your validation
        var rules = new List<IValidationRule<Person>> { new ValidatesMaxAge(), new ValidatesName() };
// test each validation rule and collect a list of failures
        var failures = rules.Select(rule => rule.Test(new Person()))
            .Where(failure => failure != null);
        var isValid = !failures.Any();
    }

    [Test]
    public void Test_II()
    {
        var testy = new TestEntity();
        var validator = new CustomValidator();
        var res = validator.Validate(testy);
        res.Should().NotBeNull();
    }
}