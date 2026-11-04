# CrossApp

Наскрізний проєкт з крос-платформного програмування.

## Предметна область

**Бібліотека**

Основні сутності:

* `Book` — книга;
* `BookCopy` — примірник;
* `Reader` — читач;
* `Loan` — видача книги.

## Структура

```text
CrossApp/
├── CrossApp.sln
├── README.md
└── src/
    ├── Core/
    │   ├── Core.csproj
    │   └── EnvironmentInfo.cs
    └── Cli/
        ├── Cli.csproj
        └── Program.cs
```

`Cli` використовує бібліотеку `Core` через `ProjectReference`.

## Збірка та запуск

```bash
dotnet build
dotnet run --project src/Cli
```

## Multi-targeting

`Core` збирається для:

```xml
<TargetFrameworks>net8.0;net9.0</TargetFrameworks>
```

Після збірки створюються окремі каталоги `net8.0` та `net9.0`.

## Публікація

### Self-contained

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
```

```bash
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true
```

### Framework-dependent

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```

Self-contained містить .NET Runtime, а framework-dependent потребує встановленого Runtime.

## Результати

| RID       | Режим               | Розмір, МБ | Runtime |
| --------- | ------------------- | ---------: | ------- |
| win-x64   | self-contained      |    70,6562 | ні      |
| linux-x64 | self-contained      |    70,5850 | ні      |
| win-x64   | framework-dependent |          — | так     |

## Додаткові завдання

**PublishSingleFile:**

```powershell
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish\win-x64-singlefile
```

**PublishTrimmed:**

```powershell
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishTrimmed=true -o publish\win-x64-trimmed
```

**Linux-x64:**

```powershell
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true -o publish\linux-x64
```

Linux-публікацію можна перевірити у Docker-контейнері `mcr.microsoft.com/dotnet/runtime-deps:8.0`.

## Лабораторна робота 4

Предметна область: Бібліотека.

Основні сутності:
- BookCopy — примірник книги;
- Loan — видача книги.

Інваріанти:
- інвентарний номер не може бути порожнім;
- назва книги не може бути порожньою;
- не можна видати вже виданий примірник;
- не можна повернути вже повернутий примірник.

Стан BookCopy змінюється тільки через методи IssueCopy()
та ReturnCopy().

Для створення сутності використовується фабричний метод Create().
Для перетворення між доменною сутністю та DTO використовуються
методи ToDto() та FromDto().

## Лабораторна робота 5 — Сервісний шар і сховища

У роботі реалізовано абстракцію сховища бібліотечних примірників через інтерфейс `IBookStore`.

Створено дві реалізації:
- `InMemoryBookStore` — зберігає примірники в оперативній пам’яті.
- `FileBookStore` — зберігає примірники у JSON-файлі.

Клас `LendingService` реалізує операції перегляду, пошуку, додавання, видачі, повернення та видалення примірників. Він залежить від інтерфейсу `IBookStore`, а не від конкретного сховища.

Вибір сховища виконується вручну в `Program.cs`:
- `dotnet run --project src/Cli` — сховище в оперативній пам’яті.
- `dotnet run --project src/Cli -- --file` — файлове сховище `data/library.json`.

Ручне впровадження залежностей реалізовано через створення вибраного сховища в точці входу та передавання його в конструктор `LendingService`. DI-контейнер не використовується.

 ## Додаткові завдання

1. Декоратор CachingBookStore

Реалізовано декоратор для кешування результатів пошуку примірників за ID. Це зменшує кількість повторних звернень до сховища.

2. Пошук Search

У класі LendingService додано метод Search, який приймає умову Func<BookCopy, bool> і дозволяє шукати примірники за різними критеріями.

3. Фабрика StoreFactory

Створено фабрику для автоматичного вибору сховища залежно від аргументів запуску програми: FileBookStore для --file або InMemoryBookStore за замовчуванням.

## Лабораторна робота 6 — Події, сповіщення та делегати

У роботі реалізовано механізм подій для відстеження змін бібліотечних примірників.

Створено:
- `BookCopyChangeKind` — перелік типів змін: додавання, видача, повернення та видалення примірника.
- `BookCopyChangedEventArgs` — клас, який зберігає інформацію про подію: ID примірника, тип зміни та час її виникнення.
- Подію `Changed` у класі `LendingService`, яка спрацьовує після виконання операцій із примірниками.

Під час зміни даних сервіс генерує подію, на яку можуть підписуватися інші частини програми та виконувати відповідну обробку.

## Додаткові завдання

1. Інтерфейс сповіщень `INotificationSink`

Створено інтерфейс `INotificationSink` із методом `Notify(string message)`.

Реалізовано дві реалізації:
- `ConsoleSink` — виводить повідомлення в консоль.
- `FileSink` — записує повідомлення у файл.

Реалізації передаються до `LendingService` через конструктор. Це приклад ручного впровадження залежностей (Manual Dependency Injection).

2. Делегат `Action<string>` для імпорту

У `BookCsvImporter` додано параметр `Action<string>? reportSkipped`, який дозволяє передавати функцію для повідомлення про пропущені рядки під час імпорту CSV.

Якщо рядок містить помилку, формується повідомлення з номером рядка та причиною пропуску. Передана функція виводить це повідомлення в консоль.

3. Асинхронні обробники подій

Розглянуто використання `async void` для асинхронних обробників подій.

Оскільки стандартний делегат `EventHandler` повертає `void`, асинхронний обробник події може використовувати `async void`.

Основні недоліки такого підходу:
- неможливо очікувати завершення обробника через `await`;
- складніше перехоплювати винятки;
- помилки можуть стати необробленими.

Для звичайних асинхронних методів рекомендується використовувати `async Task`.