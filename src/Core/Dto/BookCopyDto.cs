namespace Core.Dto;

public record BookCopyDto(
    string Id,
    string InventoryNumber,
    string Title,
    bool IsAvailable);

    // DTO для відображення примірника книги у звітах
public sealed record BookCopyLine(
    string Id,
    string InventoryNumber,
    string Title,
    bool IsAvailable
);

// DTO для статистики за доступністю книг
public sealed record AvailabilitySummary(
    string Status,
    int Count,
    double Percentage
);

// Дані про операцію з примірником для демонстрації JOIN.
public sealed record BookCopyActivity(
    string BookId,
    string Activity
);

// DTO результату об'єднання двох колекцій.
public sealed record BookCopyActivityLine(
    string BookId,
    string Title,
    string InventoryNumber,
    string Activity
);

