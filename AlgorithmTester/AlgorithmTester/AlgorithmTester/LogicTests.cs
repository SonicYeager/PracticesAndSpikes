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
        var rules = new List<IValidationRule> { new ValidatesMaxAge(), new ValidatesName() };
// test each validation rule and collect a list of failures
        var failures = rules.Select(rule => rule.Test(person))
            .Where(failure => failure != null);
        bool isValid = !failures.Any();
    }
}