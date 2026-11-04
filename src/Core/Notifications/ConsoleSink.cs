namespace Cli.Notifications;

using Core.Abstractions;

public sealed class ConsoleSink : INotificationSink
{
    public void Notify(string message)
    {
        Console.WriteLine(message);
    }
}