using System.Text;
using Core.Domain;

namespace Core.Export;

public sealed class CsvBookCopyExporter : IBookCopyExporter
{
    public void Export(IEnumerable<BookCopy> books, string path)
    {
        ArgumentNullException.ThrowIfNull(books);

        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var lines = new List<string>
        {
            "Id;InventoryNumber;Title;IsAvailable"
        };

        foreach (var book in books)
        {
            string title = book.Title.Replace("\"", "\"\"");

            lines.Add(
                $"{book.Id};{book.InventoryNumber};\"{title}\";{book.IsAvailable}");
        }

        File.WriteAllLines(path, lines, Encoding.UTF8);
    }
}