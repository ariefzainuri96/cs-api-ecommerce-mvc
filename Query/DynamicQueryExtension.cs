using System;
using System.Linq.Expressions;
using System.Reflection;
using Ecommerce.Model.Dto;

namespace Ecommerce.Query;

public static class DynamicQueryExtensions
{
    /// <summary>
    /// Dynamically applies a 'Contains' filter on a string property of an entity.
    /// </summary>
    public static IQueryable<T> ApplyDynamicFilter<T>(
        this IQueryable<T> query,
        PaginationRequestDto request) where T : class
    {
        // ----------------------------------------------------
        // FAIL-FAST VALIDATION (Ensuring input fields are safe)
        // ----------------------------------------------------
        if (string.IsNullOrWhiteSpace(request.SearchField) || string.IsNullOrWhiteSpace(request.SearchValue))
        {
            // If inputs are empty, return the original query (no filter applied)
            return query;
        }

        // ----------------------------------------------------
        // 1. Validate and Retrieve Property Info
        // ----------------------------------------------------
        Type entityType = typeof(T);
        PropertyInfo? property = entityType.GetProperty(
            request.SearchField,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        // FAIL-FAST: Property not found OR is not a string (only supports 'Contains' on strings)
        if (property == null || property.PropertyType != typeof(string))
        {
            throw new ArgumentException($"Entity '{entityType.Name}' does not have a searchable string property named '{request.SearchField}'.");
        }

        // ----------------------------------------------------
        // 2. Build the Expression Tree
        // ----------------------------------------------------
        // Parameter: (p) =>
        ParameterExpression parameter = Expression.Parameter(entityType, "p");

        // Property: p.Name (or p.Description, etc.)
        MemberExpression memberAccess = Expression.Property(parameter, property);

        // Constant: "bakso" (the search term)
        ConstantExpression constant = Expression.Constant(request.SearchValue.ToLower());

        // Method: p.Name.ToLower()
        MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        MethodCallExpression toLowerCall = Expression.Call(memberAccess, toLowerMethod);

        // Method: p.Name.ToLower().Contains(searchValue.ToLower())
        MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        MethodCallExpression body = Expression.Call(toLowerCall, containsMethod, constant);

        // Lambda: (p) => p.Name.ToLower().Contains("bakso")
        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        // ----------------------------------------------------
        // 3. Apply the Filter to IQueryable
        // ----------------------------------------------------
        return query.Where(lambda);
    }
}
