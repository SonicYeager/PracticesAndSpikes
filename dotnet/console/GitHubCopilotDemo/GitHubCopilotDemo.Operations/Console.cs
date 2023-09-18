using GitHubCopilotDemo.Contracts;

namespace GitHubCopilotDemo.Operations
{
    public class Console : IConsole
    {
        //implement interface
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}