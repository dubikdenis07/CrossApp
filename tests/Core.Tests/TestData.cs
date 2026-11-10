
using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static IReadOnlyList<BookCopy> CreateCopies(int issuedCount = 1)
    {
        var books = new List<BookCopy>
        {
            BookCopy.Create("BC-101", "INV-101", "Захар Беркут"),
            BookCopy.Create("BC-102", "INV-102", "Лісова пісня"),
            BookCopy.Create("BC-103", "INV-103", "Кайдашева сім'я")
        };

        for (int i = 0; i < issuedCount; i++)
        {
            books[i].IssueCopy();
        }

        return books;
    }
}
