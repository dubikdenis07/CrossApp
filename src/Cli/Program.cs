using Core;

EnvironmentReport report = EnvironmentInfo.Collect();

Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
Console.WriteLine("Студент: Дубик Денис, група FEI-34");
Console.WriteLine(new string('-', 52));

Console.WriteLine($"ОС                    : {report.OsDescription}");
Console.WriteLine($"Runtime               : {report.FrameworkDescription}");
Console.WriteLine($"Архітектура процесу  : {report.ProcessArchitecture}");
Console.WriteLine($"RID (визначено)       : {report.DetectedRid}");
Console.WriteLine($"RID (від .NET)        : {report.ReportedRid}");
Console.WriteLine($"Каталог застосунку    : {report.BaseDirectory}");

Console.WriteLine(new string('-', 52));
Console.WriteLine("Предметна область: Бібліотека (книги, примірники, читачі, видачі)");