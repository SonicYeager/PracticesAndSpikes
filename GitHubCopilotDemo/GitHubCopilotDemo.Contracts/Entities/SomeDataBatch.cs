namespace GitHubCopilotDemo.Contracts.Entities
{
    public record SomeDataBatch(
        int Id,
        string Name,
        string Description,
        string City,
        Coordinates Coordinates);
}