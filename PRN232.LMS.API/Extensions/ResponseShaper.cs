using System.Reflection;

namespace PRN232.LMS.API.Extensions;

public static class ResponseShaper
{
    public static IEnumerable<object> ShapeMany<TResponse>(IEnumerable<TResponse> items, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return items.Cast<object>().ToList();
        }

        var selectedProperties = GetSelectedProperties<TResponse>(fields).ToList();

        return items
            .Select(item => ShapeOne(item, selectedProperties))
            .ToList();
    }

    public static object ShapeOne<TResponse>(TResponse item, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return item as object ?? new();
        }

        return ShapeOne(item, GetSelectedProperties<TResponse>(fields).ToList());
    }

    private static Dictionary<string, object?> ShapeOne<TResponse>(
        TResponse item,
        IReadOnlyList<PropertyInfo> selectedProperties)
    {
        var shaped = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in selectedProperties)
        {
            shaped[ToCamelCase(property.Name)] = property.GetValue(item);
        }

        return shaped;
    }

    private static IEnumerable<PropertyInfo> GetSelectedProperties<TResponse>(string fields)
    {
        var requestedFields = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return typeof(TResponse)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property =>
                requestedFields.Contains(property.Name) ||
                requestedFields.Contains(ToCamelCase(property.Name)));
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || char.IsLower(value[0]))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
