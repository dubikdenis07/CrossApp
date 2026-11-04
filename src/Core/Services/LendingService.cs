
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using System.Linq;

namespace Core.Services;

public sealed class LendingService
{
    private readonly IBookStore _store;

    // Подія про зміну книжкових примірників
    public event EventHandler<BookCopyChangedEventArgs>? Changed;
    private readonly IBookStore _store;
    private readonly IReadOnlyList<INotificationSink> _sinks;

        public LendingService(
        IBookStore store,
        IEnumerable<INotificationSink>? sinks = null)
    {
        _store = store ??
            throw new ArgumentNullException(nameof(store));

        _sinks = sinks?.ToList() ?? [];
    }

    // Метод виклику події
    private void OnChanged(
    string copyId,
    BookCopyChangeKind kind)
{
    var eventArgs =
        new BookCopyChangedEventArgs(copyId, kind);

    Changed?.Invoke(this, eventArgs);

    string message =
        $"{eventArgs.At:HH:mm:ss} | {kind} | ID: {copyId}";

    foreach (var sink in _sinks)
    {
        sink.Notify(message);
    }
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

        OnChanged(item.Id, BookCopyChangeKind.BookAdded);

        return item;
    }

    public void IssueCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.IssueCopy();
        _store.Update(item);

        OnChanged(item.Id, BookCopyChangeKind.CopyIssued);
    }

    public void ReturnCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.ReturnCopy();
        _store.Update(item);

        OnChanged(item.Id, BookCopyChangeKind.CopyReturned);
    }

    public bool Remove(string id)
    {
        bool removed = _store.Remove(id);

        if (removed)
        {
            OnChanged(id, BookCopyChangeKind.CopyRemoved);
        }

        return removed;
    }

        public IReadOnlyList<BookCopy> FindAll(
        Func<BookCopy, bool> predicate)
    {
        return All()
            .Where(predicate)
            .ToList();
    }
}
