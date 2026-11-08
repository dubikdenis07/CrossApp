
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<BookCopy> BookCopies()
    {
        yield return BookCopy.Create("BC-001", "INV-001", "Кобзар");
        yield return BookCopy.Create("BC-002", "INV-002", "Захар Беркут");
        yield return BookCopy.Create("BC-003", "INV-003", "Тигролови");
        yield return BookCopy.Create("BC-004", "INV-004", "Лісова пісня");
        yield return BookCopy.Create("BC-005", "INV-005", "Місто");
        yield return BookCopy.Create("BC-006", "INV-006", "Чорна рада");
        yield return BookCopy.Create("BC-007", "INV-007", "Кайдашева сім'я");
        yield return BookCopy.Create("BC-008", "INV-008", "Енеїда");
        yield return BookCopy.Create("BC-009", "INV-009", "Маруся Чурай");
        yield return BookCopy.Create("BC-010", "INV-010", "Земля");
        yield return BookCopy.Create("BC-011", "INV-011", "Intermezzo");
        yield return BookCopy.Create("BC-012", "INV-012", "Хіба ревуть воли");
        yield return BookCopy.Create("BC-013", "INV-013", "Прапороносці");
        yield return BookCopy.Create("BC-014", "INV-014", "Тигролови");
        yield return BookCopy.Create("BC-015", "INV-015", "Вершники");
        yield return BookCopy.Create("BC-016", "INV-016", "Собор");
        yield return BookCopy.Create("BC-017", "INV-017", "Украдене щастя");
        yield return BookCopy.Create("BC-018", "INV-018", "Наталка Полтавка");    
    }
}
