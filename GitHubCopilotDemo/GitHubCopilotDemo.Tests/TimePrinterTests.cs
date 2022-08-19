using GitHubCopilotDemo.Contracts;
using GitHubCopilotDemo.Operations;
using NSubstitute;

namespace GitHubCopilotDemo.Tests;
public class TimePrinterTests
{
    //test time printer class
    //use NSubstitute to mock the time provider
    //check that time printer uses time provider now to get time
    //check that time printer uses IConsole to print time
    [Fact]
    public void PrintTime_PrintsNow()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var console = Substitute.For<IConsole>();
        var timePrinter = new TimePrinter(timeProvider, console);
        timeProvider.Now().Returns(new DateTime(2019, 1, 1, 12, 0, 0));
        timePrinter.PrintActualTime();
        console.Received().WriteLine("It is now 12:00:00");
    }
}