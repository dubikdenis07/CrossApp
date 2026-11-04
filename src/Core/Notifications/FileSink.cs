using Core.Abstractions;

namespace Cli.Notifications;

public sealed class FileSink : INotificationSink
{
    private readonly string _path;

    public FileSink(string path)
    {
        _path = Path.GetFullPath(path);

        Directory.CreateDirectory(
            Path.GetDirectoryName(_path)!);
    }

    public void Notify(string message)
    {
        File.AppendAllText(
            _path,
            message + Environment.NewLine);
    }
}