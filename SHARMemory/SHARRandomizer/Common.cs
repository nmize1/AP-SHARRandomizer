using System.Collections.Concurrent;

namespace SHARRandomizer;
public static class Common
{
    private static readonly string LogFile;
    private static readonly ConcurrentQueue<string> LogQueue;

    static Common()
    {
        LogFile = $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        LogQueue = new();

        Task.Run(async () =>
        {
            while (true)
            {
                if (!LogQueue.IsEmpty)
                {
                    try
                    {
                        using var sw = new StreamWriter(LogFile, true);
                        while (LogQueue.TryDequeue(out var log))
                            sw.WriteLine(log);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <WriteLog> Error writing log to file: {ex}");
                    }
                }

                await Task.Delay(100);
            }
        });
    }

    public static void WriteLog(object log, string method)
    {
        var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] <{method}> {log}";
        Console.WriteLine(msg);

        LogQueue.Enqueue(msg);
    }
}
