namespace Core.Dto;

public record BookCopyDto(
    string Id,
    string InventoryNumber,
    string Title,
    bool IsAvailable);