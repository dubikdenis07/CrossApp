
using Core;
using Core.Dto;
using Core.Import;
using Core.Domain;
using Core.Services;
using Core.Abstractions;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ЛАБОРАТОРНА РОБОТА 3: ІМПОРТ CSV / JSON

string importPath = args.FirstOrDefault(
    arg => !arg.StartsWith("--", StringComparison.Ordinal))
    ?? Path.Combine("data", "sample.csv");

bool useFile = args.Contains(
    "--file",
    StringComparer.OrdinalIgnoreCase);

bool skipImport = args.Contains(
    "--no-import",
    StringComparer.OrdinalIgnoreCase);

if (!skipImport)
{
    Console.WriteLine("=== Імпорт CSV / JSON ===");

    if (!File.Exists(importPath))
    {
        Console.WriteLine(
            $"Файл не знайдено: {Path.GetFullPath(importPath)}");
    }
    else
    {
        string extension =
            Path.GetExtension(importPath).ToLowerInvariant();

        switch (extension)
        {
            case ".csv":
            {
                ImportResult<BookDto> result =
                    BookCsvImporter.Load(importPath);

                int total = result.Items.Count + result.Errors.Count;
                int accepted = result.Items.Count;
                int skipped = result.Errors.Count;

                double errorPercent = total == 0
                    ? 0
                    : skipped * 100.0 / total;

                Console.WriteLine(
                    $"Імпорт: усього {total}, прийнято {accepted}, " +
                    $"пропущено {skipped}, помилки {errorPercent:F1}%");

                Console.WriteLine(
                    $"Завантажено книг: {result.Items.Count}");

                foreach (BookDto importedBook in result.Items.Take(5))
                {
                    Console.WriteLine(
                        $"  {importedBook.Id,-6} " +
                        $"{importedBook.Isbn,-18} " +
                        $"{importedBook.Title,-25} " +
                        $"{importedBook.Year}");
                }

                if (result.Errors.Count > 0)
                {
                    Console.WriteLine(
                        $"Пропущено рядків: {result.Errors.Count}");

                    foreach (string error in result.Errors)
                    {
                        Console.WriteLine($"  ! {error}");
                    }
                }

                break;
            }

            case ".json":
            {
                ImportResult<BookDto> result =
                    BookJsonImporter.Load(importPath);

                Console.WriteLine(
                    $"Завантажено книг: {result.Items.Count}");

                foreach (BookDto importedBook in result.Items.Take(5))
                {
                    Console.WriteLine(
                        $"  {importedBook.Id,-6} " +
                        $"{importedBook.Isbn,-18} " +
                        $"{importedBook.Title,-25} " +
                        $"{importedBook.Year}");
                }

                if (result.Errors.Count > 0)
                {
                    Console.WriteLine(
                        $"Помилок імпорту: {result.Errors.Count}");

                    foreach (string error in result.Errors)
                    {
                        Console.WriteLine($"  ! {error}");
                    }
                }

                break;
            }

            default:
                Console.WriteLine(
                    $"Непідтримуваний формат: {extension}");
                break;
        }
    }
}
else
{
    Console.WriteLine("Імпорт пропущено через аргумент --no-import.");
}

// ЛАБОРАТОРНА РОБОТА 4: ДОДАТКОВЕ

Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 1: імпорт у BookCopy ===");

if (!skipImport && File.Exists(importPath))
{
    string extension =
        Path.GetExtension(importPath).ToLowerInvariant();

    ImportResult<BookDto>? importResult = extension switch
    {
        ".csv" => BookCsvImporter.Load(importPath),
        ".json" => BookJsonImporter.Load(importPath),
        _ => null
    };

    if (importResult is not null)
    {
        BookCopyImportResult converted =
            BookCopyImportService.Convert(importResult);

        Console.WriteLine(
            $"Створено сутностей: {converted.Items.Count}");

        Console.WriteLine(
            $"Помилок: {converted.Errors.Count}");

        foreach (BookCopy copy in converted.Items.Take(5))
        {
            Console.WriteLine(
                $"  {copy.Id} | {copy.InventoryNumber} | {copy.Title}");
        }

        foreach (string error in converted.Errors)
        {
            Console.WriteLine($"  ! {error}");
        }
    }
}

// ЛАБОРАТОРНА РОБОТА 4: ДОМЕННА МОДЕЛЬ ТА ІНВАРІАНТИ

Console.WriteLine();
Console.WriteLine("=== Сценарій 1: успішна робота BookCopy ===");

BookCopy libraryBook = BookCopy.Create(
    "BC-DEMO-001",
    "INV-DEMO-001",
    "Кобзар");

Console.WriteLine(
    $"Створено: {libraryBook.Title}, " +
    $"інвентарний номер: {libraryBook.InventoryNumber}");

Console.WriteLine($"Доступна: {libraryBook.IsAvailable}");

libraryBook.IssueCopy();

Console.WriteLine(
    $"Після видачі: доступна = {libraryBook.IsAvailable}");

libraryBook.ReturnCopy();

Console.WriteLine(
    $"Після повернення: доступна = {libraryBook.IsAvailable}");

BookCopyDto dto = libraryBook.ToDto();

Console.WriteLine();
Console.WriteLine("=== ToDto / FromDto ===");

Console.WriteLine(
    $"DTO: {dto.Id}, {dto.InventoryNumber}, " +
    $"{dto.Title}, доступна = {dto.IsAvailable}");

BookCopy restoredBook = BookCopy.FromDto(dto);

Console.WriteLine(
    $"Відновлено: {restoredBook.Title}, " +
    $"доступна = {restoredBook.IsAvailable}");

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

TryDo(
    "повторна видача",
    () =>
    {
        libraryBook.IssueCopy();
        libraryBook.IssueCopy();
    });

libraryBook.ReturnCopy();

TryDo(
    "порожній інвентарний номер",
    () =>
    {
        BookCopy.Create(
            "BC-DEMO-002",
            "",
            "Чистий аркуш");
    });

TryDo(
    "порожня назва книги",
    () =>
    {
        BookCopy.Create(
            "BC-DEMO-003",
            "INV-DEMO-003",
            "");
    });

TryDo(
    "повторне повернення",
    () =>
    {
        libraryBook.ReturnCopy();
    });

// ЛАБОРАТОРНА РОБОТА 5: СЕРВІСНИЙ ШАР І ДВА СХОВИЩА

Console.WriteLine();
Console.WriteLine("=== Лабораторна робота 5: сховища ===");

string dataPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "data",
    "library.json");

IBookStore store = StoreFactory.Create(args);

store = new CachingBookStore(store);

LendingService service = new LendingService(store);

Console.WriteLine($"Вибране сховище: {store.GetType().Name}");

if (useFile)
{
    Console.WriteLine($"Файл даних: {dataPath}");
}
Console.WriteLine($"Вибране сховище: {store.GetType().Name}");

if (useFile)
{
    Console.WriteLine($"Файл даних: {dataPath}");
}

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 — Показати всі примірники");
    Console.WriteLine("2 — Знайти примірник за ID");
    Console.WriteLine("3 — Додати примірник");
    Console.WriteLine("4 — Видати примірник");
    Console.WriteLine("5 — Повернути примірник");
    Console.WriteLine("6 — Видалити примірник");
    Console.WriteLine("7 — Показати тільки доступні примірники");
    Console.WriteLine("0 — Завершити роботу");
    Console.Write("Ваш вибір: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
            {
                IReadOnlyList<BookCopy> items = service.All();

                if (items.Count == 0)
                {
                    Console.WriteLine("Список примірників порожній.");
                    break;
                }

                foreach (BookCopy item in items)
                {
                    PrintBook(item);
                }

                Console.WriteLine($"Усього примірників: {items.Count}");
                break;
            }

            case "2":
            {
                Console.Write("Введіть ID: ");
                string id = Console.ReadLine() ?? "";

                BookCopy? item = service.Find(id);

                if (item is null)
                {
                    Console.WriteLine("Примірник не знайдено.");
                }
                else
                {
                    PrintBook(item);
                }

                break;
            }

            case "3":
            {
                Console.Write("ID: ");
                string id = Console.ReadLine() ?? "";

                Console.Write("Інвентарний номер: ");
                string inventoryNumber = Console.ReadLine() ?? "";

                Console.Write("Назва книги: ");
                string title = Console.ReadLine() ?? "";

                BookCopy item = service.AddBook(
                    id,
                    inventoryNumber,
                    title);

                Console.WriteLine("Примірник успішно додано:");
                PrintBook(item);
                break;
            }

            case "4":
            {
                Console.Write("ID примірника для видачі: ");
                string id = Console.ReadLine() ?? "";

                service.IssueCopy(id);

                Console.WriteLine("Примірник успішно видано.");
                break;
            }

            case "5":
            {
                Console.Write("ID примірника для повернення: ");
                string id = Console.ReadLine() ?? "";

                service.ReturnCopy(id);

                Console.WriteLine("Примірник успішно повернуто.");
                break;
            }

            case "6":
            {
                Console.Write("ID примірника для видалення: ");
                string id = Console.ReadLine() ?? "";

                bool removed = service.Remove(id);

                Console.WriteLine(
                    removed
                        ? "Примірник видалено."
                        : "Примірник не знайдено.");

                break;
            }

            case "7":
            {
                IReadOnlyList<BookCopy> available =
                    service.All()
                        .Where(book => book.IsAvailable)
                        .ToList();

                Console.WriteLine(
                    $"Доступних примірників: {available.Count}");

                foreach (BookCopy item in available)
                {
                    PrintBook(item);
                }

                break;
            }

            case "0":
                running = false;
                Console.WriteLine("Роботу завершено.");
                break;

            default:
                Console.WriteLine("Невідома команда.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

static void PrintBook(BookCopy item)
{
    Console.WriteLine(
        $"ID: {item.Id} | " +
        $"Інв. номер: {item.InventoryNumber} | " +
        $"Назва: {item.Title} | " +
        $"Статус: {(item.IsAvailable ? "У бібліотеці" : "Видано")}");
}

static void TryDo(string title, Action action)
{
    try
    {
        action();

        Console.WriteLine(
            $"{title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"{title}: {ex.GetType().Name} — {ex.Message}");
    }
}
