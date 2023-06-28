using AlgorithmTester.Objects;
using FluentValidation;

public class CustomValidator : AbstractValidator<TestEntity>
{
    public CustomValidator()
    {
        RuleFor(x => x.Id).Configure(cfg =>
        {
            cfg.MessageBuilder = context =>
            {
                context.MessageFormatter
                    .AppendPropertyValue(context.PropertyValue)
                    .AppendPropertyName(context.PropertyName); // Default behaviour uses context.DisplayName, by changing it to context.PropertyName this uses the full path.
                return context.GetDefaultMessage();
            };
        });
        RuleForEach(e => e.Collection).SetValidator(new CollectionValidator());
    }
}

public class CollectionValidator : AbstractValidator<FlatObject>
{
    public CollectionValidator()
    {
        RuleFor(x => x.Weight).Configure(cfg =>
        {
            cfg.MessageBuilder = context =>
            {
                context.MessageFormatter
                    .AppendPropertyValue(context.PropertyValue)
                    .AppendPropertyName(context.PropertyName);
                return context.GetDefaultMessage();
            };
        }).GreaterThan(5);
    }
}