
using Core;
using Core.Domain;

namespace Core.Tests;

public class BookCopyDomainTests
{
    // Тест 8: не можна повторно видати вже виданий примірник
    [Fact]
    public void IssueCopy_WhenAlreadyIssued_ThrowsDomainException()
    {
        var book = BookCopy.Create(
            "BC-104",
            "INV-104",
            "Тіні забутих предків");

        book.IssueCopy();

        Assert.Throws<DomainException>(
            () => book.IssueCopy());
    }
}
