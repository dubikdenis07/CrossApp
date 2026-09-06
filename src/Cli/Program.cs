using System;
using System.Runtime.InteropServices;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool jsonMode = args.Contains("--json");

string osDescription = RuntimeInformation.OSDescription;
string environmentOs = Environment.OSVersion.ToString();
string architecture = RuntimeInformation.ProcessArchitecture.ToString();
string dotnetVersion = Environment.Version.ToString();
string runtime = RuntimeInformation.FrameworkDescription;
string appDirectory = AppContext.BaseDirectory;
string currentDirectory = Environment.CurrentDirectory;

if (jsonMode)
{
    var information = new
    {
        OSDescription = osDescription,
        EnvironmentOS = environmentOs,
        Architecture = architecture,
        DotNetVersion = dotnetVersion,
        Runtime = runtime,
        AppDirectory = appDirectory,
        CurrentDirectory = currentDirectory
    };

    Console.WriteLine(JsonSerializer.Serialize(information));
}
else
{
    Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
    Console.WriteLine("Студент: Дубик Денис, група FEI-34");
    Console.WriteLine(new string('-', 52));

    Console.WriteLine($"ОС (OSDescription)   : {osDescription}");
    Console.WriteLine($"ОС (Environment)     : {environmentOs}");
    Console.WriteLine($"Архітектура процесу  : {architecture}");
    Console.WriteLine($"Версія .NET (CLR)    : {dotnetVersion}");
    Console.WriteLine($"Runtime              : {runtime}");
    Console.WriteLine($"Каталог застосунку   : {appDirectory}");
    Console.WriteLine($"Поточний каталог     : {currentDirectory}");

    Console.WriteLine(new string('-', 52));
    Console.WriteLine("Предметна область: Бібліотека (книги, примірники, читачі, видачі)");
}