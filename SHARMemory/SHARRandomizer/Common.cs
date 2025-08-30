using System.Collections.Concurrent;
using SHARMemory.SHAR.Structs;

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

    public static Vector3 GetVector3Dir(Vector3 pos1, Vector3 pos2)
    {
        float dx = pos2.X - pos1.X;
        float dy = pos2.Y - pos1.Y;
        float dz = pos2.Z - pos1.Z;

        float length = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);

        if(length == 0) return new Vector3(0, 0, 0);

        return new Vector3(dx / length, dy / length, dz / length);
    }
}
