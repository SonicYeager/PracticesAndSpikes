using GitHubCopilotDemo.Contracts;

namespace GitHubCopilotDemo.Operations
{
    //use and implement ITimeProvider
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now() => DateTime.Now;
    }
}