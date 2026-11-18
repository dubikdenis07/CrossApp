using Core.Domain;
using Core.Storage;

namespace Core.Tests;

public sealed class FileBookStoreTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _filePath;

    public FileBookStoreTests()
    {
        _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CrossAppTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_tempDirectory);

        _filePath = Path.Combine(
            _tempDirectory,
            "library.json");
    }

    [Fact]
    public void Add_NewStore_ListReturnsSavedBookCopy()
    {
        // Arrange
        var book = BookCopy.Create(
            "TEST-001",
            "INV-TEST-001",
            "Тестова книга");

        // Act
        var firstStore = new FileBookStore(_filePath);
        firstStore.Add(book);

        var secondStore = new FileBookStore(_filePath);
        var result = secondStore.List();

        // Assert
        Assert.Single(result);
        Assert.Equal("TEST-001", result[0].Id);
        Assert.Equal("INV-TEST-001", result[0].InventoryNumber);
        Assert.Equal("Тестова книга", result[0].Title);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(
                _tempDirectory,
                recursive: true);
        }
    }
}