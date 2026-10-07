using Core.Domain;
using Core.Dto;
using Core.Import;

namespace Core.Services;

public record BookCopyImportResult(
    List<BookCopy> Items,
    List<string> Errors);

public static class BookCopyImportService
{
    public static BookCopyImportResult Convert(
        ImportResult<BookDto> importResult)
    {
        if (importResult is null)
            throw new ArgumentNullException(nameof(importResult));

        List<BookCopy> items = new();
        List<string> errors = new(importResult.Errors);

        foreach (BookDto book in importResult.Items)
        {
            try
            {
                BookCopy copy = BookCopy.Create(
                    book.Id,
                    $"INV-{book.Id}",
                    book.Title);

                items.Add(copy);
            }
            catch (Exception ex)
            {
                errors.Add(
                    $"Книга {book.Id}: {ex.Message}");
            }
        }

        return new BookCopyImportResult(
            items,
            errors);
    }
}