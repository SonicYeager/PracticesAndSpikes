namespace HotChocolate.Checker.GraphQL.Types;

public class User
{
    [IsProjected(true)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
}