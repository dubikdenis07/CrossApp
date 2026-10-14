using Core.Abstractions;

namespace Core.Storage;

public static class StoreFactory
{
    public static IBookStore Create(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        bool useFile = args.Contains(
            "--file",
            StringComparer.OrdinalIgnoreCase);

        if (useFile)
        {
            string dataPath = Path.Combine(
                "data",
                "library.json");

            return new FileBookStore(dataPath);
        }

        return new InMemoryBookStore(
            SampleData.BookCopies());
    }
}