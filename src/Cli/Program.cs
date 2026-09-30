using Core.Dto;
using Core.Import;

//MixedImporter.Load("data/mixed.csv");

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

switch (extension)
{
    case ".csv":
    {
        ImportResult<BookDto> result = BookCsvImporter.Load(path);
        int total = result.Items.Count + result.Errors.Count;
int accepted = result.Items.Count;
int skipped = result.Errors.Count;
double errorPercent = total == 0 ? 0 : skipped * 100.0 / total;

Console.WriteLine(
    $"Імпорт: усього {total}, прийнято {accepted}, " +
    $"пропущено {skipped}, помилки {errorPercent:F1}%");

        Console.WriteLine($"Завантажено книг: {result.Items.Count}");

        foreach (BookDto book in result.Items.Take(5))
            Console.WriteLine(
                $"  {book.Id,-6} {book.Isbn,-18} {book.Title,-25} {book.Year}");

        if (result.Errors.Count > 0)
        {
            Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");

            foreach (string error in result.Errors)
                Console.WriteLine($"  ! {error}");
        }

        break;
    }

    case ".json":
    {
        ImportResult<BookDto> result = BookJsonImporter.Load(path);

        Console.WriteLine($"Завантажено книг: {result.Items.Count}");

        foreach (BookDto book in result.Items.Take(5))
            Console.WriteLine(
                $"  {book.Id,-6} {book.Isbn,-18} {book.Title,-25} {book.Year}");

        break;
    }

    default:
        Console.WriteLine($"Непідтримуваний формат: {extension}");
        return 1;
}

return 0;