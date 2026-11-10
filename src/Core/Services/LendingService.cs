
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Import;

namespace Core.Services;

public sealed class LendingService
{
    private readonly IBookStore _store;
    private readonly IReadOnlyList<INotificationSink> _sinks;

    public event EventHandler<BookCopyChangedEventArgs>? Changed;

    public LendingService(
        IBookStore store,
        IEnumerable<INotificationSink>? sinks = null)
    {
        _store = store ??
            throw new ArgumentNullException(nameof(store));

        _sinks = sinks?.ToList() ?? [];
    }

    // Виклик події та надсилання сповіщень
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

    // Отримання всіх примірників
    public IReadOnlyList<BookCopy> All()
    {
        return _store.List();
    }

    // Пошук примірника за ID
    public BookCopy? Find(string id)
    {
        return _store.GetById(id);
    }

    // Пошук за умовою
    public IEnumerable<BookCopy> Search(
        Func<BookCopy, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _store.List().Where(predicate);
    }

    // Додавання книги
    public BookCopy AddBook(
        string id,
        string inventoryNumber,
        string title)
    {
        BookCopy item = BookCopy.Create(
            id,
            inventoryNumber,
            title);

        _store.Add(item);

        OnChanged(item.Id, BookCopyChangeKind.BookAdded);

        return item;
    }

    // Видача примірника
    public void IssueCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.IssueCopy();

        _store.Update(item);

        OnChanged(item.Id, BookCopyChangeKind.CopyIssued);
    }

    // Повернення примірника
    public void ReturnCopy(string id)
    {
        BookCopy item = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Примірник з ID={id} не знайдений.");

        item.ReturnCopy();

        _store.Update(item);

        OnChanged(item.Id, BookCopyChangeKind.CopyReturned);
    }

    // Видалення примірника
    public bool Remove(string id)
    {
        bool removed = _store.Remove(id);

        if (removed)
        {
            OnChanged(id, BookCopyChangeKind.CopyRemoved);
        }

        return removed;
    }

    // Пошук усіх примірників за умовою
    public IEnumerable<BookCopy> FindAll(
        Func<BookCopy, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _store.List().Where(predicate);
    }

    // Асинхронний імпорт із відображенням прогресу
    public async Task<int> ImportAsync(
        IBookCopyImporter importer,
        string path,
        CancellationToken ct = default,
        IProgress<int>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(importer);

        ct.ThrowIfCancellationRequested();

        IReadOnlyList<BookCopy> copies =
            await importer.LoadAsync(path, ct);

        int added = 0;
        int processed = 0;

        foreach (BookCopy copy in copies)
        {
            ct.ThrowIfCancellationRequested();

            if (_store.GetById(copy.Id) is null)
            {
                _store.Add(copy);

                OnChanged(
                    copy.Id,
                    BookCopyChangeKind.BookAdded);

                added++;
            }

            processed++;

            int percentage = copies.Count == 0
                ? 100
                : (int)(processed * 100.0 / copies.Count);

            progress?.Report(percentage);
        }

        if (copies.Count == 0)
        {
            progress?.Report(100);
        }

        return added;
    }

    // Асинхронний імпорт зі зв'язаними токенами та таймаутом
    public async Task<int> ImportWithTimeoutAsync(
        IBookCopyImporter importer,
        string path,
        TimeSpan timeout,
        CancellationToken userToken = default,
        IProgress<int>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(importer);

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeout),
                "Таймаут повинен бути більшим за нуль.");
        }

        using var timeoutCts =
            new CancellationTokenSource();

        timeoutCts.CancelAfter(timeout);

        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                userToken,
                timeoutCts.Token);

        try
        {
            return await ImportAsync(
                importer,
                path,
                linkedCts.Token,
                progress);
        }
        catch (OperationCanceledException)
            when (timeoutCts.IsCancellationRequested &&
                  !userToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Імпорт перевищив таймаут " +
                $"{timeout.TotalSeconds} секунд.");
        }
    }
}
