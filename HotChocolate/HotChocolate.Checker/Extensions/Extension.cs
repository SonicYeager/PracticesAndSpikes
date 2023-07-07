using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate.Data.Projections.Expressions;
using HotChocolate.Resolvers;

namespace HotChocolate.Checker.Extensions;

public static class AutoMapperQueryableExtensions
{
    public static IQueryable<TDest> ProjectToFix<TSource, TDest>(this IQueryable<TSource> query, IResolverContext context)
    {
        var mapper = context.Service<IMapper>();

        context.LocalContextData =
            context.LocalContextData.SetItem(QueryableProjectionProvider.SkipProjectionKey,
                true);

        var visitorContext =
            new QueryableProjectionContext(
                context,
                context.ObjectType,
                context.Selection.Field.Type.UnwrapRuntimeType(), false);
        var visitor = new QueryableProjectionVisitor();
        visitor.Visit(visitorContext);

#pragma warning disable CS8631
        Expression<Func<TDest, object?>> projection = visitorContext.Project<TDest, object?>();
#pragma warning restore CS8631

        List<Expression<Func<TDest, object?>>> membersToExpand = new();

        VisitRoot(projection, membersToExpand);

        return query.ProjectTo(mapper.ConfigurationProvider, membersToExpand.ToArray());
    }

    private static void VisitRoot<TDest>(Expression<Func<TDest, object?>> root, List<Expression<Func<TDest, object?>>> expressions)
    {
        var lambdaBody = (UnaryExpression)root.Body;

        var memberInit = (MemberInitExpression)lambdaBody.Operand;

        const int level = 0;

        var parameter = MakeParameter(memberInit.Type, level);

        expressions.AddRange(VisitMemberInit(memberInit, parameter, level)
            .Select(expression => Expression.Lambda<Func<TDest, object?>>(expression, parameter)));
    }

    private static IEnumerable<Expression> VisitMemberInit(MemberInitExpression memberInit,
        Expression path, int level)
    {
        var memberAssignments = memberInit.Bindings.Cast<MemberAssignment>();

        foreach (var expression in memberAssignments.SelectMany(memberAssignment => VisitMemberAssignment(memberAssignment, path, level)))
        {
            yield return expression;
        }
    }

    private static IEnumerable<Expression> VisitMemberAssignment(MemberAssignment memberAssignment, Expression path, int level)
    {
        var memberExpression = memberAssignment.Expression;

        var member = (PropertyInfo)memberAssignment.Member;

        switch (memberExpression)
        {
            case MemberExpression:
                yield return VisitPrimitive(member, path);
                break;
            case MemberInitExpression nestedMemberInit:
            {
                foreach (var expression in VisitNestedObject(nestedMemberInit, member, path, level))
                {
                    yield return expression;
                }

                break;
            }
            case MethodCallExpression toArrayCallExpr:
            {
                foreach (var expression in VisitNestedCollection(toArrayCallExpr, member, path, level))
                {
                    yield return expression;
                }

                break;
            }
        }
    }

    private static Expression VisitPrimitive(PropertyInfo member, Expression path)
    {
        Expression memberAccess = Expression.MakeMemberAccess(path, member);

        if (member is { PropertyType.IsValueType: true }) memberAccess = Expression.Convert(memberAccess, typeof(object));

        return memberAccess;
    }

    private static IEnumerable<Expression> VisitNestedObject(MemberInitExpression nestedMemberInit, PropertyInfo member, Expression path,
        int level)
    {
        var nestedPath = Expression.MakeMemberAccess(path, member);

        foreach (var expression in VisitMemberInit(nestedMemberInit, nestedPath, level))
        {
            yield return expression;
        }
    }

    private static IEnumerable<Expression> VisitNestedCollection(MethodCallExpression toArrayCall, PropertyInfo member, Expression path,
        int level)
    {
        if (toArrayCall.Arguments[0] is not MethodCallExpression selectCall)
            yield break;

        if (selectCall.Arguments[1] is not LambdaExpression selectLambda)
            yield break;

        var nestedPath = Expression.MakeMemberAccess(path, member);

        var memberInit = (MemberInitExpression)selectLambda.Body;

        level++;

        var parameter = MakeParameter(memberInit.Type, level);

        var selectMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Single(static mi => mi.Name == nameof(Enumerable.Select) &&
                                 mi.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2)
            .MakeGenericMethod(parameter.Type, typeof(object));

        foreach (var lambda in VisitMemberInit(memberInit, parameter, level).Select(expression => Expression.Lambda(expression, parameter)))
        {
            yield return Expression.Call(null, selectMethod, nestedPath, lambda);
        }
    }

    private static ParameterExpression MakeParameter(Type type, int lambdaLevel)
    {
        return Expression.Parameter(type, "p_" + lambdaLevel);
    }
}