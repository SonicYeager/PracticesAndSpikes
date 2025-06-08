namespace FindACoach.Records;

public sealed record Coach(int Id, string FirstName, string LastName, ISet<string> Areas, string Description, int HourlyRate);