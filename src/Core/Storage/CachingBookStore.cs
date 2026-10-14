using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class CachingBookStore : IBookStore
{
    private readonly IBookStore _inner;
    private readonly Dictionary<string, BookCopy> _cache = new();

    public CachingBookStore(IBookStore inner)
    {
        _inner = inner
            ?? throw new ArgumentNullException(nameof(inner));
    }

    public IReadOnlyList<BookCopy> List()
    {
        return _inner.List();
    }

    public BookCopy? GetById(string id)
    {
        if (_cache.TryGetValue(id, out BookCopy? cached))
            return cached;

        BookCopy? book = _inner.GetById(id);

        if (book is not null)
            _cache[id] = book;

        return book;
    }

    public void Add(BookCopy book)
    {
        _inner.Add(book);
        _cache.Clear();
    }

    public void Update(BookCopy book)
    {
        _inner.Update(book);
        _cache.Clear();
    }

    public bool Remove(string id)
    {
        bool removed = _inner.Remove(id);
        _cache.Remove(id);
        return removed;
    }
}