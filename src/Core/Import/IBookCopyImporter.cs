
using Core.Domain;

namespace Core.Import;

public interface IBookCopyImporter
{
    Task<IReadOnlyList<BookCopy>> LoadAsync(
        string path,
        CancellationToken ct = default);
}
