namespace Core.Events;

public sealed class BookCopyChangedEventArgs : EventArgs
{
    public string CopyId { get; }
    public BookCopyChangeKind Kind { get; }
    public DateTimeOffset At { get; }

    public BookCopyChangedEventArgs(
        string copyId,
        BookCopyChangeKind kind)
    {
        CopyId = copyId;
        Kind = kind;
        At = DateTimeOffset.Now;
    }
}