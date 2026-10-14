
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class InMemoryBookStore : IBookStore
{
    private readonly Dictionary<string, BookCopy> _items;

    public InMemoryBookStore(IEnumerable<BookCopy>? seed = null)
    {
        _items = new Dictionary<string, BookCopy>(
            StringComparer.OrdinalIgnoreCase);

        if (seed is not null)
        {
            foreach (BookCopy item in seed)
            {
                if (!_items.TryAdd(item.Id, item))
                {
                    throw new InvalidOperationException(
                        $"Примірник з ID={item.Id} вже існує.");
                }
            }
        }
    }

    public IReadOnlyList<BookCopy> List()
    {
        return _items.Values.ToList();
    }

    public BookCopy? GetById(string id)
    {
        _items.TryGetValue(id, out BookCopy? item);
        return item;
    }

    public void Add(BookCopy item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!_items.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException(
                $"Примірник з ID={item.Id} вже існує.");
        }
    }

    public void Update(BookCopy item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!_items.ContainsKey(item.Id))
        {
            throw new InvalidOperationException(
                $"Примірник з ID={item.Id} не знайдений.");
        }

        _items[item.Id] = item;
    }

    public bool Remove(string id)
    {
        return _items.Remove(id);
    }
}
