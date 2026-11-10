
using Core.Domain;
using Core.Import;
using Core.Services;
using Core.Storage;

namespace Core.Tests;

public class AsyncImportExtraTests
{
    [Fact]
    public async Task ImportWithTimeout_WhenTimeoutExpires_ThrowsTimeoutException()
    {
        // Arrange
        var service = new LendingService(new InMemoryBookStore());

        var importer = new NeverCompletingImporter();

        // Act
        var exception = await Assert.ThrowsAsync<TimeoutException>(
            () => service.ImportWithTimeoutAsync(
                importer,
                "test.json",
                TimeSpan.FromMilliseconds(200)));

        // Assert
        Assert.Contains("таймаут", exception.Message);
    }

    private sealed class NeverCompletingImporter : IBookCopyImporter
    {
        public async Task<IReadOnlyList<BookCopy>> LoadAsync(
            string path,
            CancellationToken ct = default)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, ct);

            return Array.Empty<BookCopy>();
        }
    }

[Fact]
public async Task ImportAsync_ReportsProgress()
{
    // Arrange
    var service = new LendingService(new InMemoryBookStore());

    var copies = new[]
    {
        BookCopy.Create("T1", "INV-T1", "Книга 1"),
        BookCopy.Create("T2", "INV-T2", "Книга 2"),
        BookCopy.Create("T3", "INV-T3", "Книга 3")
    };

    var importer = new FixedImporter(copies);

    var percentages = new List<int>();

    var progress = new InlineProgress<int>(
        value => percentages.Add(value));

    // Act
    int added = await service.ImportAsync(
        importer,
        "test.json",
        CancellationToken.None,
        progress);

    // Assert
    Assert.Equal(3, added);

    Assert.Equal(
        new[] { 33, 66, 100 },
        percentages);
}

private sealed class FixedImporter : IBookCopyImporter
{
    private readonly IReadOnlyList<BookCopy> _copies;

    public FixedImporter(IReadOnlyList<BookCopy> copies)
    {
        _copies = copies;
    }

    public Task<IReadOnlyList<BookCopy>> LoadAsync(
        string path,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return Task.FromResult(_copies);
    }
}

private sealed class InlineProgress<T> : IProgress<T>
{
    private readonly Action<T> _callback;

    public InlineProgress(Action<T> callback)
    {
        _callback = callback;
    }

    public void Report(T value)
    {
        _callback(value);
    }
}

}
