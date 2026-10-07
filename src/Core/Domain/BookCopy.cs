using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    private bool _isAvailable;

    public string Id { get; }
    public string InventoryNumber { get; }
    public string Title { get; }
    public bool IsAvailable => _isAvailable;

    private BookCopy(
        string id,
        string inventoryNumber,
        string title,
        bool isAvailable)
    {
        Id = id;
        InventoryNumber = inventoryNumber;
        Title = title;
        _isAvailable = isAvailable;
    }

    public static BookCopy Create(
        string id,
        string inventoryNumber,
        string title)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Ідентифікатор примірника обов'язковий",
                nameof(id));

        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException(
                "Інвентарний номер не може бути порожнім",
                nameof(inventoryNumber));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Назва книги не може бути порожньою",
                nameof(title));

        return new BookCopy(
            id.Trim(),
            inventoryNumber.Trim(),
            title.Trim(),
            true);
    }

    public void IssueCopy()
    {
        if (!_isAvailable)
            throw new InvalidOperationException(
                $"Примірник {InventoryNumber} вже виданий");

        _isAvailable = false;
    }

    public void ReturnCopy()
    {
        if (_isAvailable)
            throw new InvalidOperationException(
                $"Примірник {InventoryNumber} вже знаходиться в бібліотеці");

        _isAvailable = true;
    }

    public BookCopyDto ToDto()
    {
        return new BookCopyDto(
            Id,
            InventoryNumber,
            Title,
            IsAvailable);
    }

    public static BookCopy FromDto(BookCopyDto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        BookCopy copy = Create(
            dto.Id,
            dto.InventoryNumber,
            dto.Title);

        if (!dto.IsAvailable)
            copy.IssueCopy();

        return copy;
    }
}