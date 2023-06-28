namespace AlgorithmTester.Logic;

public class Person
{
    public int Age { get; set; }
    public string Name { get; set; }

    public List<string> Collection { get; set; }
}

public class ValidationFailure
{
    public ValidationFailure(string message, string path)
    {
        Message = message;
        Path = path;
    }

    public string Message { get; set; }
    public string Path { get; set; }
}

public interface IValidationRule<TEntity>
{
    ValidationFailure Test(TEntity entity);
}

public class ValidatesMaxAge : IValidationRule<Person>
{
    public ValidationFailure Test(Person entity)
    {
        if (entity.Age > 100)
            return new ValidationFailure("Age is too high.", nameof(entity.Age));
        else
        {
            return null;
        }
    }
}

public class ValidatesName : IValidationRule<Person>
{
    public ValidationFailure Test(Person entity)
    {
        if (entity.Collection.Count >= 0)
            return new ValidationFailure("Can not be empty.", FieldPathCreator<Person>.GetPath(e => e.Collection[1]));
        else
        {
            throw new NotImplementedException();
        }
    }
}