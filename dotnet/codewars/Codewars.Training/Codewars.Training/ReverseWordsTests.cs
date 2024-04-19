using Codewars.Training.ReverseWords;

namespace Codewars.Training;

[TestFixture]
public class ReverseWordsTests
{
    [Test]
    public void ReverseWordsTest()
    {
        Assert.That(Kata.ReverseWords("This is an example!"), Is.EqualTo("sihT si na !elpmaxe"));
    }
    
    [Test]
    public void ReverseWordsTest_WithDoubleSpaces_RetainsTheCountOfSpaces()
    {
        Assert.That(Kata.ReverseWords("This  is  an  example!"), Is.EqualTo("sihT  si  na  !elpmaxe"));
    }
}