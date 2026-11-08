using System.Diagnostics;
using Core.Domain;

namespace Core.Services;

public sealed record BenchmarkResult(
    string Variant,
    long ElapsedMilliseconds,
    int ResultCount
);

public class LinqBenchmarkService
{
    public IReadOnlyList<BenchmarkResult> Run(int recordCount = 100_000)
    {
        if (recordCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(recordCount));
        }

        // Генеруємо тестові записи.
        List<BookCopy> books = Enumerable
            .Range(1, recordCount)
            .Select(i =>
            {
                var book = BookCopy.Create(
                    $"BC-{i:D6}",
                    $"INV-{i:D6}",
                    $"Книга {i:D6}"
                );

                if (i % 3 == 0)
                {
                    book.IssueCopy();
                }

                return book;
            })
            .ToList();

        // Прогрів запиту перед вимірюванням.
        _ = books
            .Where(book => book.IsAvailable)
            .OrderBy(book => book.Title)
            .Take(5)
            .ToList();

        // Варіант 1: з проміжним ToList().
        var stopwatch = Stopwatch.StartNew();

        List<BookCopy> resultWithToList = new();

        for (int i = 0; i < 10; i++)
        {
            resultWithToList = books
                .Where(book => book.IsAvailable)
                .ToList()
                .OrderBy(book => book.Title)
                .Take(5)
                .ToList();
        }

        stopwatch.Stop();

        long timeWithToList = stopwatch.ElapsedMilliseconds;

        // Варіант 2: без проміжного ToList().
        stopwatch.Restart();

        List<BookCopy> resultWithoutToList = new();

        for (int i = 0; i < 10; i++)
        {
            resultWithoutToList = books
                .Where(book => book.IsAvailable)
                .OrderBy(book => book.Title)
                .Take(5)
                .ToList();
        }

        stopwatch.Stop();

        long timeWithoutToList = stopwatch.ElapsedMilliseconds;

        return
        [
            new BenchmarkResult(
                "З проміжним ToList()",
                timeWithToList,
                resultWithToList.Count
            ),
            new BenchmarkResult(
                "Без проміжного ToList()",
                timeWithoutToList,
                resultWithoutToList.Count
            )
        ];
    }
}