using Core.Domain;

namespace Core.Export;

public interface IBookCopyExporter
{
    void Export(IEnumerable<BookCopy> books, string path);
}