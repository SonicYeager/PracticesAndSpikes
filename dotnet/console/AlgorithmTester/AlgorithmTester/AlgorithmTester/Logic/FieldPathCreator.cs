using System.Linq.Expressions;

namespace AlgorithmTester.Logic;

public static class FieldPathCreator<TType>
{
    public static string GetPath<TProperty>(Expression<Func<TType, TProperty>> expr)
    {
        var name = expr.Parameters[0].Name;

        return expr.ToString()
            .Replace($"{name} => {name}", typeof(TType).Name);
    }
}

// usage: ReflectionHelper<MyClass>.GetPath(x => x.Foo.Blah) // -> "MyClass.Foo.Blah"
