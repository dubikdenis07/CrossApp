using Core.Domain;
using Core.Dto;

namespace Core.Services;

public static class BookCopyReportService
{
    public static IReadOnlyList<BookCopyLine> Available(
        IEnumerable<BookCopy> source)
    {
        return source
            .Where(book => book.IsAvailable)
            .OrderBy(book => book.Title)
            .ThenBy(book => book.InventoryNumber)
            .Select(book => new BookCopyLine(
                book.Id,
                book.InventoryNumber,
                book.Title,
                book.IsAvailable))
            .ToList();
    }

    public static IReadOnlyList<BookCopyLine> Issued(
        IEnumerable<BookCopy> source)
    {
        return source
            .Where(book => !book.IsAvailable)
            .OrderBy(book => book.Title)
            .ThenBy(book => book.InventoryNumber)
            .Select(book => new BookCopyLine(
                book.Id,
                book.InventoryNumber,
                book.Title,
                book.IsAvailable))
            .ToList();
    }

    public static IReadOnlyList<BookCopyLine> TopByTitle(
        IEnumerable<BookCopy> source, int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return source
            .OrderBy(book => book.Title, StringComparer.OrdinalIgnoreCase)
            .ThenBy(book => book.InventoryNumber, StringComparer.Ordinal)
            .Take(count)
            .Select(book => new BookCopyLine(
                book.Id,
                book.InventoryNumber,
                book.Title,
                book.IsAvailable))
            .ToList();
    }

    public static IReadOnlyList<AvailabilitySummary> ByAvailability(
        IEnumerable<BookCopy> source)
    {
        var items = source.ToList();
        int total = items.Count;

        return items
            .GroupBy(book => book.IsAvailable ? "Доступні" : "Видано")
            .Select(group => new AvailabilitySummary(
                group.Key,
                group.Count(),
                total == 0 ? 0 : group.Count() * 100.0 / total))
            .OrderBy(summary => summary.Status)
            .ToList();
    }

    public static (
        int Total,
        int Available,
        int Issued,
        double AvailabilityPercent)
        Totals(IEnumerable<BookCopy> source)
    {
        var items = source.ToList();

        int total = items.Count;
        int available = items.Count(book => book.IsAvailable);
        int issued = items.Count(book => !book.IsAvailable);

        double percentage = total == 0
            ? 0
            : available * 100.0 / total;

        return (total, available, issued, percentage);
    }

    public static IReadOnlyList<BookCopyLine> FindByTitle(
        IEnumerable<BookCopy> source, string fragment)
    {
        return source
            .Where(book => book.Title.Contains(
                fragment, StringComparison.OrdinalIgnoreCase))
            .OrderBy(book => book.Title)
            .Select(book => new BookCopyLine(
                book.Id,
                book.InventoryNumber,
                book.Title,
                book.IsAvailable))
            .ToList();
    }

    public static IReadOnlyList<BookCopyActivityLine> JoinActivities(
    IEnumerable<BookCopy> books,
    IEnumerable<BookCopyActivity> activities)
{
    var result =
        from book in books
        join activity in activities
            on book.Id equals activity.BookId
        select new BookCopyActivityLine(
            book.Id,
            book.Title,
            book.InventoryNumber,
            activity.Activity);

    return result.ToList();
}

    public static IReadOnlyList<AvailabilitySummary> GroupByAvailabilityQuery(
    IEnumerable<BookCopy> books)
{
    int total = books.Count();

    var result =
        from book in books
        group book by (
            book.IsAvailable ? "Доступні" : "Видано"
        ) into bookGroup
        orderby bookGroup.Key
        select new AvailabilitySummary(
            bookGroup.Key,
            bookGroup.Count(),
            total == 0
                ? 0
                : bookGroup.Count() * 100.0 / total
        );

    return result.ToList();
}

public static IReadOnlyList<AvailabilitySummary> GroupByAvailabilityMethod(
    IEnumerable<BookCopy> books)
{
    var source = books.ToList();
    int total = source.Count;

    return source
        .GroupBy(book => book.IsAvailable ? "Доступні" : "Видано")
        .OrderBy(group => group.Key)
        .Select(group => new AvailabilitySummary(
            group.Key,
            group.Count(),
            total == 0
                ? 0
                : group.Count() * 100.0 / total
        ))
        .ToList();
}
}