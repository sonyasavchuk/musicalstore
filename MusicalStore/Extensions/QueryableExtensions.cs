using System.Linq.Expressions;

namespace MusicalStore.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> expression, bool condition)
    {
        return condition ? source.Where(expression) : source;
    }
}
