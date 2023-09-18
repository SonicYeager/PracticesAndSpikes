using GitHubCopilotDemo.Contracts;

namespace GitHubCopilotDemo.Operations
{
    public class TimePrinter : ITimePrinter
    {
        /// <summary>
        /// Prints the current time.
        /// </summary>
        /// <param name="timeProvider"></param>
        /// <param name="console"></param>
        public TimePrinter(ITimeProvider timeProvider, IConsole console)
        {
            this.timeProvider = timeProvider;
            this.console = console;
        }
        public void PrintActualTime()
        {
            console.WriteLine(timeProvider.Now().ToString());
        }
        private readonly ITimeProvider timeProvider;
        private readonly IConsole console;
    }
}