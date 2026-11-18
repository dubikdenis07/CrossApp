using System.Text.Json;
using Core.Domain;
using Core.Dto;

namespace Core.Export;

public sealed class JsonBookCopyExporter : IBookCopyExporter
{
    public void Export(IEnumerable<BookCopy> books, string path)
    {
        ArgumentNullException.ThrowIfNull(books);

        var dtos = books.Select(book => book.ToDto()).ToList();

        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(
            dtos,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(path, json);
    }
}