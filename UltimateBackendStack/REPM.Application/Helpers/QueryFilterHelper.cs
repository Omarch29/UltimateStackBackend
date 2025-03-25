using System.Linq.Expressions;
using REPM.Application.Filters;

namespace REPM.Application.Helpers;

public static class QueryFilterHelper
{
    public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, IFilter filters)
    {
        if (filters == null) return query;

        var parameter = Expression.Parameter(typeof(T), "p");
        var expressions = new List<Expression>();

        foreach (var prop in filters.GetType().GetProperties())
        {
            var filterValue = prop.GetValue(filters);
            if (filterValue == null) continue; // Skip if no value is provided

            // ✅ Try to get the property from the main entity first
            var entityProperty = typeof(T).GetProperty(prop.Name);

            if (entityProperty == null)
            {
                // 🔍 Check if it's inside a navigation property (like Address.City)
                foreach (var navProperty in typeof(T).GetProperties())
                {
                    var subProperty = navProperty.PropertyType.GetProperty(prop.Name);
                    if (subProperty != null)
                    {
                        entityProperty = subProperty;
                        break;
                    }
                }
            }

            if (entityProperty == null) continue; // Skip if the property doesn't exist

            // ✅ Build the expression dynamically
            var left = Expression.Property(parameter, entityProperty);
            var right = Expression.Constant(filterValue);
            Expression condition = null;

            // Handle special cases (strings, numbers, ranges)
            if (filterValue is string)
            {
                condition = Expression.Equal(left, right);
            }
            else if (filterValue is int || filterValue is decimal)
            {
                if (prop.Name.StartsWith("Min"))
                {
                    condition = Expression.GreaterThanOrEqual(left, right);
                }
                else if (prop.Name.StartsWith("Max"))
                {
                    condition = Expression.LessThanOrEqual(left, right);
                }
                else
                {
                    condition = Expression.Equal(left, right);
                }
            }

            if (condition != null)
            {
                expressions.Add(condition);
            }
        }

        if (expressions.Count == 0) return query;

        var finalExpression = expressions.Aggregate(Expression.AndAlso);
        var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);

        return query.Where(lambda);
    }
}