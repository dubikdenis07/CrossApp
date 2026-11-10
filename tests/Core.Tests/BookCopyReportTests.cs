
using Core.Services;

namespace Core.Tests;

public class BookCopyReportTests
{
    // Тест 1, 2 і 3: перевірка підрахунку примірників
    [Theory]
    [InlineData(0, 3, 0, 100.0)]
    [InlineData(1, 2, 1, 200.0 / 3.0)]
    [InlineData(2, 1, 2, 100.0 / 3.0)]
    public void Totals_ReturnsCorrectCounts(
        int issuedCount,
        int expectedAvailable,
        int expectedIssued,
        double expectedPercent)
    {
        var books = TestData.CreateCopies(issuedCount);

        var result = BookCopyReportService.Totals(books);

        Assert.Equal(3, result.Total);
        Assert.Equal(expectedAvailable, result.Available);
        Assert.Equal(expectedIssued, result.Issued);
        Assert.Equal(expectedPercent, result.AvailabilityPercent, 5);
    }

    // Тест 4: перевірка сортування за назвою
    [Fact]
    public void TopByTitle_ReturnsBooksInSortedOrder()
    {
        var books = TestData.CreateCopies();

        var result = BookCopyReportService.TopByTitle(books, 3);

        Assert.Equal(3, result.Count);
        Assert.Equal("Захар Беркут", result[0].Title);
        Assert.Equal("Кайдашева сім'я", result[1].Title);
        Assert.Equal("Лісова пісня", result[2].Title);
    }

    // Тест 5: перевірка від'ємної кількості
    [Fact]
    public void TopByTitle_WithNegativeCount_ThrowsException()
    {
        var books = TestData.CreateCopies();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => BookCopyReportService.TopByTitle(books, -1));
    }

    // Тест 6: перевірка звіту доступних примірників
    [Fact]
    public void Available_ReturnsOnlyAvailableBooks()
    {
        var books = TestData.CreateCopies(1);

        var result = BookCopyReportService.Available(books);

        Assert.Equal(2, result.Count);
        Assert.All(result, book => Assert.True(book.IsAvailable));
    }

    // Тест 7: перевірка звіту виданих примірників
    [Fact]
    public void Issued_ReturnsOnlyIssuedBooks()
    {
        var books = TestData.CreateCopies(1);

        var result = BookCopyReportService.Issued(books);

        Assert.Single(result);
        Assert.False(result[0].IsAvailable);
        Assert.Equal("Захар Беркут", result[0].Title);
    }
}
