
using System.Runtime.CompilerServices;
using System.Text.Json;
using Core.Domain;
using Core.Dto;

namespace Core.Import;

public sealed class JsonBookCopyImporter : IBookCopyImporter
{
    public async Task<IReadOnlyList<BookCopy>> LoadAsync(
        string path,
        CancellationToken ct = default)
    {
        var result = new List<BookCopy>();

        await foreach (BookCopy copy in ImportAsync(path, ct))
        {
            result.Add(copy);
        }

        return result;
    }

    public async IAsyncEnumerable<BookCopy> ImportAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);

        List<BookCopyDto>? dtos =
            await JsonSerializer.DeserializeAsync<List<BookCopyDto>>(
                stream,
                options,
                cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        if (dtos is null)
        {
            throw new DomainException(
                "JSON-файл не містить списку примірників.");
        }

        for (int i = 0; i < dtos.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (dtos[i] is null)
            {
                throw new DomainException(
                    $"Запис №{i + 1} не може бути порожнім.");
            }

            BookCopy copy;

            try
            {
                copy = BookCopy.FromDto(dtos[i]);
            }
            catch (ArgumentException ex)
            {
                throw new DomainException(
                    $"Помилка в записі №{i + 1}: {ex.Message}",
                    ex);
            }

            yield return copy;
        }
    }
}
