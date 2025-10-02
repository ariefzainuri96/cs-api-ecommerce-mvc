using System.Linq.Expressions;
using System.Reflection;
using Ecommerce.Model.Dto;

namespace Ecommerce.Query;

public static class DynamicQueryExtensions
{
    /// <summary>
    /// Dynamically applies a 'Contains' filter on a string property, supporting dot-notation for joined entities (e.g., "Product.Name").
    /// </summary>
    public static IQueryable<T> ApplyDynamicFilter<T>(
        this IQueryable<T> query,
        PaginationRequestDto requestDto) where T : class
    {
        // ----------------------------------------------------
        // FAIL-FAST VALIDATION
        // ----------------------------------------------------
        if (string.IsNullOrWhiteSpace(requestDto.SearchField) || string.IsNullOrWhiteSpace(requestDto.SearchValue))
        {
            return query;
        }

        string[] propertyNames;

        if (requestDto.SearchField.Contains('.'))
        {
            propertyNames = requestDto.SearchField.Split('.');
        }
        else
        {
            propertyNames = [requestDto.SearchField];
        }

        Type currentType = typeof(T);
        ParameterExpression parameter = Expression.Parameter(currentType, "p");
        Expression memberAccess = parameter;

        // ----------------------------------------------------
        // 1. Traverse the Property Path (Handles Joins)
        // ----------------------------------------------------
        foreach (var propertyName in propertyNames)
        {
            PropertyInfo? property = currentType.GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // FAIL-FAST: Property not found
            if (property == null)
            {
                throw new ArgumentException($"Invalid property path: Property '{propertyName}' not found on type '{currentType.Name}'.");
            }

            // Move to the next member in the path
            memberAccess = Expression.Property(memberAccess, property);
            currentType = property.PropertyType;
        }

        // ----------------------------------------------------
        // 2. Final Type Check (Must be string for Contains method)
        // ----------------------------------------------------
        if (currentType != typeof(string))
        {
            // FAIL-FAST: Cannot use 'Contains' on a non-string type (e.g., if path ended on Product).
            throw new ArgumentException($"The final property in the path ('{requestDto.SearchField}') must be a string for 'Contains' filtering.");
        }

        // ----------------------------------------------------
        // 3. Build the Contains Expression
        // ----------------------------------------------------
        ConstantExpression constant = Expression.Constant(requestDto.SearchValue.ToLower());

        // Call .ToLower() on the final property (e.g., Product.Name.ToLower())
        MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        MethodCallExpression toLowerCall = Expression.Call(memberAccess, toLowerMethod);

        // Call .Contains(searchValue)
        MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        MethodCallExpression body = Expression.Call(toLowerCall, containsMethod, constant);

        // Build the final Lambda: (p) => p.Product.Name.ToLower().Contains(searchValue.ToLower())
        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        // ----------------------------------------------------
        // 4. Apply the Filter
        // ----------------------------------------------------
        return query.Where(lambda);
    }

    /// <summary>
    /// Dynamically applies an OrderBy or OrderByDescending clause based on a property path (supports joins).
    /// </summary>
    public static IQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> query,
        PaginationRequestDto requestDto) where T : class
    {
        // ----------------------------------------------------
        // FAIL-FAST VALIDATION
        // ----------------------------------------------------
        if (string.IsNullOrWhiteSpace(requestDto.OrderBy))
        {
            // If no property is specified, return the original query
            return query;
        }

        // Determine the generic method name to call (OrderBy or OrderByDescending)
        string methodName = requestDto.OrderDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending" : "OrderBy";

        // ----------------------------------------------------
        // 1. Build the Property Access Expression (Handles Joins)
        // ----------------------------------------------------
        string[] properties;

        if (requestDto.OrderBy.Contains('.'))
        {
            properties = requestDto.OrderBy.Split('.');
        }
        else
        {
            properties = [requestDto.OrderBy];
        }

        Type currentType = typeof(T);
        ParameterExpression parameter = Expression.Parameter(currentType, "p");
        Expression memberAccess = parameter;

        // Traverse the path (e.g., from ShoppingCart to Product.Name)
        foreach (var propertyName in properties)
        {
            PropertyInfo? property = currentType.GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // FAIL-FAST: Property not found
            if (property == null)
            {
                throw new ArgumentException($"Invalid OrderBy property: '{requestDto.OrderBy}' - '{propertyName}' not found on type '{currentType.Name}'.");
            }

            memberAccess = Expression.Property(memberAccess, property);
            currentType = property.PropertyType;
        }

        // ----------------------------------------------------
        // 2. Build the Method Call (OrderBy / OrderByDescending)
        // ----------------------------------------------------
        // Create the Lambda expression: p => p.Property
        LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

        // The IQueryable.OrderBy method is static, so we get it from Queryable class
        MethodInfo orderByMethod = typeof(Queryable).GetMethods()
            .Single(method => method.Name == methodName
                && method.IsGenericMethodDefinition
                && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), memberAccess.Type); // T and the property's type (e.g., string, int)

        // ----------------------------------------------------
        // 3. Apply the Ordering
        // ----------------------------------------------------
        // Call Queryable.OrderBy<T, TProperty>(query, orderByLambda)
        return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, orderByLambda })!;
    }
}

