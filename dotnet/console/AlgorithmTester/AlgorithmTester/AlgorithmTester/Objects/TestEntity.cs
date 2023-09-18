namespace AlgorithmTester.Objects;

public class TestEntity
{
    public int Id { get; set; } = 10;

    public List<FlatObject> Collection { get; set; } = new List<FlatObject> { new() };
}