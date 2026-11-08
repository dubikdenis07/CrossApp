
using Core.Domain;
using Core.Dto;
using Core.Services;
using Core.Extensions;

internal static class ReportConsole
{
    public static bool Run(LendingService service, string[] args)
    {
        string? command = args
            .Select(arg => arg.ToLowerInvariant())
            .FirstOrDefault(arg => arg is
                "report-available" or
                "report-issued" or
                "report-top" or
                "report-top-title" or
                "report-top-inventory" or
                "report-groups" or
                "report-sum" or
                "report-search" or
                "report-deferred" or
                "report-query" or
                "report-method" or
                "report-join" or
                "benchmark");

        if (command is null)
            return false;

        IReadOnlyList<BookCopy> books = service.All();

        switch (command)
        {
            case "report-available":
                Console.WriteLine("\n=== ДОСТУПНІ КНИГИ ===");
                Print(BookCopyReportService.Available(books));
                break;

            case "report-issued":
                Console.WriteLine("\n=== ВИДАНІ КНИГИ ===");
                Print(BookCopyReportService.Issued(books));
                break;

            case "report-top":
            case "report-top-title":
            {
                Console.WriteLine("\n=== ТОП-5 КНИГ ЗА НАЗВОЮ ===");

                var result = books.TopBy(
                    book => book.Title.Length, 5);

                foreach (BookCopy book in result)
                    PrintBook(book);

                break;
            }

            case "report-top-inventory":
            {
                Console.WriteLine(
                    "\n=== ТОП-5 ЗА ДОВЖИНОЮ ІНВЕНТАРНОГО НОМЕРА ===");

                var result = books.TopBy(
                    book => book.InventoryNumber.Length, 5);

                foreach (BookCopy book in result)
                    PrintBook(book);

                break;
            }

            case "report-groups":
                Console.WriteLine("\n=== ГРУПУВАННЯ ЗА СТАТУСОМ ===");

                foreach (AvailabilitySummary item
                    in BookCopyReportService.ByAvailability(books))
                {
                    Console.WriteLine(
                        $"{item.Status,-15} " +
                        $"Кількість: {item.Count,3} " +
                        $"Частка: {item.Percentage:F2}%");
                }
                break;

            case "report-sum":
            {
                Console.WriteLine("\n=== ЗАГАЛЬНА СТАТИСТИКА ===");

                var totals = BookCopyReportService.Totals(books);

                Console.WriteLine($"Усього примірників: {totals.Total}");
                Console.WriteLine($"Доступних: {totals.Available}");
                Console.WriteLine($"Видано: {totals.Issued}");
                Console.WriteLine(
                    $"Відсоток доступних: {totals.AvailabilityPercent:F2}%");

                break;
            }

            case "report-search":
            {
                Console.Write("\nВведіть частину назви книги: ");

                string fragment = Console.ReadLine() ?? "";

                Print(BookCopyReportService.FindByTitle(
                    books, fragment));

                break;
            }

            case "report-deferred":
                DeferredExecutionDemo(service);
                break;

            case "report-query":
            {
                Console.WriteLine(
                    "\n=== ГРУПУВАННЯ: QUERY SYNTAX ===");

                foreach (AvailabilitySummary item
                    in BookCopyReportService.GroupByAvailabilityQuery(books))
                {
                    Console.WriteLine(
                        $"{item.Status,-15} " +
                        $"Кількість: {item.Count,3} " +
                        $"Частка: {item.Percentage:F2}%");
                }

                break;
            }

            case "report-method":
            {
                Console.WriteLine(
                    "\n=== ГРУПУВАННЯ: METHOD SYNTAX ===");

                foreach (AvailabilitySummary item
                    in BookCopyReportService.GroupByAvailabilityMethod(books))
                {
                    Console.WriteLine(
                        $"{item.Status,-15} " +
                        $"Кількість: {item.Count,3} " +
                        $"Частка: {item.Percentage:F2}%");
                }

                break;
            }

            case "report-join":
            {
                Console.WriteLine("\n=== JOIN КНИГ ТА АКТИВНОСТЕЙ ===");

                List<BookCopyActivity> activities =
                [
                    new("BC-001", "Видача"),
                    new("BC-002", "Повернення"),
                    new("BC-005", "Видача")
                ];

                var result =
                    BookCopyReportService.JoinActivities(
                        books, activities);

                foreach (BookCopyActivityLine item in result)
                {
                    Console.WriteLine(
                        $"{item.BookId} | {item.Title} | " +
                        $"{item.InventoryNumber} | {item.Activity}");
                }

                Console.WriteLine($"Усього результатів: {result.Count}");

                break;
            }

            case "benchmark":
            {
                Console.WriteLine("\n=== ПОРІВНЯННЯ ПРОДУКТИВНОСТІ LINQ ===");

                LinqBenchmarkService benchmark = new();

                foreach (BenchmarkResult item in benchmark.Run())
                {
                    Console.WriteLine(
                        $"{item.Variant}: " +
                        $"{item.ElapsedMilliseconds} мс, " +
                        $"результатів: {item.ResultCount}");
                }

                break;
            }
        }

        return true;
    }

    private static void Print(IReadOnlyList<BookCopyLine> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }

        foreach (BookCopyLine book in books)
        {
            string status = book.IsAvailable
                ? "Доступна"
                : "Видана";

            Console.WriteLine(
                $"ID: {book.Id,-12} | " +
                $"Інв. номер: {book.InventoryNumber,-14} | " +
                $"Назва: {book.Title,-25} | " +
                $"Статус: {status}");
        }

        Console.WriteLine($"Усього записів: {books.Count}");
    }

    private static void PrintBook(BookCopy book)
    {
        Console.WriteLine(
            $"ID: {book.Id,-12} | " +
            $"Інв. номер: {book.InventoryNumber,-14} | " +
            $"Назва: {book.Title,-25} | " +
            $"Статус: {(book.IsAvailable ? "Доступна" : "Видана")}");
    }

    private static void DeferredExecutionDemo(LendingService service)
    {
        Console.WriteLine("\n=== ВІДКЛАДЕНЕ ВИКОНАННЯ LINQ ===");

        List<BookCopy> source = service.All().ToList();

        var query = source.Where(book =>
            book.Title.Contains(
                "Тимчасова",
                StringComparison.OrdinalIgnoreCase));

        Console.WriteLine(
            $"Кількість до зміни джерела: {query.Count()}");

        source.Add(BookCopy.Create(
            "BC-TEMP-001",
            "INV-TEMP-001",
            "Тимчасова книга"));

        Console.WriteLine(
            $"Кількість після зміни джерела: {query.Count()}");

        List<BookCopy> snapshot = query.ToList();

        source.Clear();

        Console.WriteLine(
            $"Кількість у знімку після очищення джерела: {snapshot.Count}");

        Console.WriteLine(
            "Демонстрацію відкладеного виконання завершено.");
    }
}
