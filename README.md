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