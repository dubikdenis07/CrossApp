namespace Core.Extensions;

public static class EnumerableExtensions
{
    public static IReadOnlyList<T> TopBy<T>(
        this IEnumerable<T> source,
        Func<T, int> selector,
        int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return source
            .OrderByDescending(selector)
            .Take(count)
            .ToList();
    }
}