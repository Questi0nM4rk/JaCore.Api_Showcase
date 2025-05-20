using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // Needs System.Linq.Dynamic.Core nuget package
using System.Reflection;
using System.Linq.Expressions;

namespace JaCore.Api.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string? sortBy)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default sort: typically by a primary key or a ModifiedAt/CreatedAt field descending
                // Check if T has 'ModifiedAt', then 'CreatedAt', then 'Id'
                var type = typeof(T);
                if (type.GetProperty("ModifiedAt") != null) return source.OrderBy($"ModifiedAt descending");
                if (type.GetProperty("CreatedAt") != null) return source.OrderBy($"CreatedAt descending");
                if (type.GetProperty("Id") != null) return source.OrderBy($"Id ascending"); // Ascending for Id often makes sense
                return source; // No default sort applicable
            }

            // Example: "Name asc, DateOfBirth desc"
            // System.Linq.Dynamic.Core handles parsing this string.
            try
            {
                 return source.OrderBy(sortBy);
            }
            catch (Exception ex) // Catch parsing errors from Dynamic Linq
            {
                // Log the error, and fall back to default sort or throw
                // For now, falling back to no sort (or could be default sort)
                Console.WriteLine($"Error applying sort '{sortBy}': {ex.Message}"); // Replace with actual logging
                 var type = typeof(T);
                if (type.GetProperty("ModifiedAt") != null) return source.OrderBy($"ModifiedAt descending");
                if (type.GetProperty("CreatedAt") != null) return source.OrderBy($"CreatedAt descending");
                if (type.GetProperty("Id") != null) return source.OrderBy($"Id ascending");
                return source;
            }
        }

        public static IQueryable<T> IncludeProperties<T>(this IQueryable<T> source, string? includeProperties) where T : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(includeProperties))
            {
                return source;
            }

            var properties = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var property in properties)
            {
                // Basic validation: check if property exists on the type T to prevent arbitrary string injection if not using EF Core safe includes.
                // EF Core's Include method is generally safe against SQL injection with property paths.
                // However, validating property names can prevent errors for invalid include strings.
                if (IsValidPropertyPath<T>(property.Trim()))
                {
                    source = source.Include(property.Trim());
                }
                else
                {
                    // Log invalid include property request
                    Console.WriteLine($"Invalid include property path: {property.Trim()} for type {typeof(T).Name}");
                }
            }
            return source;
        }

        private static bool IsValidPropertyPath<T>(string propertyPath) where T : class
        {
            var type = typeof(T);
            PropertyInfo? currentProperty = null;
            foreach (var part in propertyPath.Split('.'))
            {
                currentProperty = type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (currentProperty == null)
                    return false;
                type = currentProperty.PropertyType;
            }
            return true;
        }

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string? searchTerm, string[] searchableProperties)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(searchTerm) || searchableProperties == null || !searchableProperties.Any())
            {
                return source;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? orExpression = null;

            foreach (var propertyName in searchableProperties)
            {
                if (string.IsNullOrWhiteSpace(propertyName)) continue;

                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null || property.PropertyType != typeof(string)) // Only apply search to string properties for this basic implementation
                {
                    // Log or handle non-string or non-existent properties if necessary
                    Console.WriteLine($"Search property '{propertyName}' not found or not a string property on type '{typeof(T).Name}'. Skipping.");
                    continue;
                }

                var member = Expression.Property(parameter, property);
                var termConstant = Expression.Constant(searchTerm.Trim(), typeof(string));
                
                // Using EF.Functions.Like for SQL LIKE, assuming source is IQueryable from EF Core
                // For non-EF providers or in-memory, Contains might be more appropriate or a different method.
                // EF.Functions.Like requires Microsoft.EntityFrameworkCore.Relational
                // However, to keep it more generic for now, let's use string.Contains which translates to LIKE in EF Core for most databases.
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                if (containsMethod == null) continue; // Should not happen for string

                // Create expression for: x.PropertyName.ToLower().Contains(searchTerm.ToLower())
                var toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
                if (toLowerMethod == null) continue;

                var memberLower = Expression.Call(member, toLowerMethod);
                var termLowerConstant = Expression.Constant(searchTerm.Trim().ToLower(), typeof(string));
                var individualExpression = Expression.Call(memberLower, containsMethod, termLowerConstant);


                if (orExpression == null)
                {
                    orExpression = individualExpression;
                }
                else
                {
                    orExpression = Expression.OrElse(orExpression, individualExpression);
                }
            }

            if (orExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);
                return source.Where(lambda);
            }

            return source;
        }
    }
} 