
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<BookCopy> BookCopies()
    {
        yield return BookCopy.Create(
            "BC-001", "INV-001", "Кобзар");

        yield return BookCopy.Create(
            "BC-002", "INV-002", "Захар Беркут");

        yield return BookCopy.Create(
            "BC-003", "INV-003", "Тигролови");

        yield return BookCopy.Create(
            "BC-004", "INV-004", "Лісова пісня");

        yield return BookCopy.Create(
            "BC-005", "INV-005", "Місто");
    }
}
