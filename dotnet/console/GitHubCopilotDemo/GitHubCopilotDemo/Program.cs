// See https://aka.ms/new-console-template for more information

using GitHubCopilotDemo.Operations;
using DemoConsole = GitHubCopilotDemo.Operations.Console;

//instantiate and use time printer to print the time, instantiate IConsole and ITimeProvider beforehand
var timePrinter = new TimePrinter(new TimeProvider(), new DemoConsole());
timePrinter.PrintActualTime();