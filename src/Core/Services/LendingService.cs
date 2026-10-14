
using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class LendingService
{
    private readonly IBookStore _store;

    public LendingService(IBookStore store)
    {
        _store = store ??
            throw new ArgumentNullException(nameof(store));
    }

    public IReadOnlyList<BookCopy> All()
    {
        return _store.List();
    }

    public BookCopy? Find(string id)
    {
        return _store.GetById(id);
    }

    public IEnumerable<BookCopy> Search(
        Func<BookCopy, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _store.List().Where(predicate);
    }

    public BookCopy AddBook(
        string id,
        string inventoryNumber,
        string title)
    {
        BookCopy item = BookCopy.Create(
            id, inventoryNumber, title);

        _store.Add(item);

        return item;
    }

    public void IssueCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.IssueCopy();
        _store.Update(item);
    }

    public void ReturnCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.ReturnCopy();
        _store.Update(item);
    }

    public bool Remove(string id)
    {
        return _store.Remove(id);
    }
}
