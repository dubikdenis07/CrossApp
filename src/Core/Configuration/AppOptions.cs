
namespace Core.Configuration;

public sealed class AppOptions
{
    public StorageOptions Storage { get; init; } = new();

    public LoggingOptions Logging { get; init; } = new();

    public string EnvironmentName { get; init; } = "Development";

    public bool TryValidate(out string error)
    {
        if (string.IsNullOrWhiteSpace(Storage.DataPath))
        {
            error = "Помилка конфігурації: Storage:DataPath не може бути порожнім.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Storage.FileName) ||
            !Storage.FileName.EndsWith(
                ".json", StringComparison.OrdinalIgnoreCase))
        {
            error = "Помилка конфігурації: Storage:FileName повинен мати розширення .json.";
            return false;
        }

        if (Storage.Format is not ("json" or "csv"))
        {
            error = "Помилка конфігурації: Storage:Format повинен бути json або csv.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}

public sealed class StorageOptions
{
    public string DataPath { get; init; } = "data";

    public string FileName { get; init; } = "library.json";

    public string Format { get; init; } = "json";

    public string ResolveFullPath(string baseDir)
    {
        var root = Path.IsPathRooted(DataPath)
            ? DataPath
            : Path.Combine(baseDir, DataPath);

        Directory.CreateDirectory(root);

        return Path.GetFullPath(Path.Combine(root, FileName));
    }
}

public sealed class LoggingOptions
{
    public string MinimumLevel { get; init; } = "Information";
}
