using Core.Dto;
using Core.Import;
using Core.Domain;
using Core.Services;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
}
else
{
    string extension = Path.GetExtension(path).ToLowerInvariant();

    switch (extension)
    {
        case ".csv":
        {
            ImportResult<BookDto> result = BookCsvImporter.Load(path);

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
                    Console.WriteLine($"  ! {error}");
            }

            break;
        }

        case ".json":
        {
            ImportResult<BookDto> result =
                BookJsonImporter.Load(path);

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

            break;
        }

        default:
            Console.WriteLine(
                $"Непідтримуваний формат: {extension}");
            break;
    }
}

Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 1 ===");

if (File.Exists(path))
{
    string extension = Path.GetExtension(path).ToLowerInvariant();

    ImportResult<BookDto>? importResult = extension switch
    {
        ".csv" => BookCsvImporter.Load(path),
        ".json" => BookJsonImporter.Load(path),
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

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("=== Сценарій 1: успішна робота ===");

BookCopy libraryBook = BookCopy.Create(
    "BC-001",
    "INV-001",
    "Кобзар");

Console.WriteLine(
    $"Створено: {libraryBook.Title}, " +
    $"інвентарний номер: {libraryBook.InventoryNumber}");

Console.WriteLine(
    $"Доступна: {libraryBook.IsAvailable}");

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
            "BC-002",
            "",
            "Чистий аркуш");
    });

TryDo(
    "порожня назва книги",
    () =>
    {
        BookCopy.Create(
            "BC-003",
            "INV-003",
            "");
    });

TryDo(
    "повторне повернення",
    () =>
    {
        libraryBook.ReturnCopy();
    });

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