
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileBookStore : IBookStore
{
    private readonly string _path;

    private readonly Dictionary<string, BookCopy> _items =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonSerializerOptions _jsonOptions =
        new() { WriteIndented = true };

    private bool _loaded;

    public FileBookStore(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(
                "Шлях до файлу не може бути порожнім.",
                nameof(path));
        }

        _path = Path.GetFullPath(path);
    }

    private void EnsureLoaded()
    {
        if (_loaded)
            return;

        _items.Clear();

        if (File.Exists(_path))
        {
            string json = File.ReadAllText(_path);

            List<BookCopyDto> dtos =
                JsonSerializer.Deserialize<List<BookCopyDto>>(
                    json, _jsonOptions) ?? new List<BookCopyDto>();

            foreach (BookCopyDto dto in dtos)
            {
                BookCopy item = BookCopy.FromDto(dto);

                if (!_items.TryAdd(item.Id, item))
                {
                    throw new InvalidDataException(
                        $"У файлі знайдено повторний ID={item.Id}.");
                }
            }
        }

        _loaded = true;
    }

    private void Flush()
    {
        string? directory = Path.GetDirectoryName(_path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        List<BookCopyDto> dtos = _items.Values
            .Select(item => item.ToDto())
            .ToList();

        string json = JsonSerializer.Serialize(dtos, _jsonOptions);

        File.WriteAllText(_path, json);
    }

    public IReadOnlyList<BookCopy> List()
    {
        EnsureLoaded();
        return _items.Values.ToList();
    }

    public BookCopy? GetById(string id)
    {
        EnsureLoaded();

        _items.TryGetValue(id, out BookCopy? item);
        return item;
    }

    public void Add(BookCopy item)
    {
        EnsureLoaded();
        ArgumentNullException.ThrowIfNull(item);

        if (!_items.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException(
                $"Примірник з ID={item.Id} вже існує.");
        }

        Flush();
    }

    public void Update(BookCopy item)
    {
        EnsureLoaded();
        ArgumentNullException.ThrowIfNull(item);

        if (!_items.ContainsKey(item.Id))
        {
            throw new InvalidOperationException(
                $"Примірник з ID={item.Id} не знайдений.");
        }

        _items[item.Id] = item;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();

        bool removed = _items.Remove(id);

        if (removed)
            Flush();

        return removed;
    }
}
