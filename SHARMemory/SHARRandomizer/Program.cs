using Newtonsoft.Json;
using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Structs;
using SHARRandomizer;
using SHARRandomizer.Classes;
using System.Diagnostics;
using System.Drawing;

string VERSION = "Alpha 0.1.10";

Console.Title = $"SHAR AP Version {VERSION}";

try
{
    using HttpClient _http = new();
    _http.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/vnd.github+json");
    _http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "archipelago");
    var latestReleaseJson = await _http.GetStringAsync("https://api.github.com/repos/nmize1/AP-SHARRandomizer/releases/latest");
    var latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(latestReleaseJson);
    if (latestRelease != null && latestRelease.Name != VERSION)
    {
        Console.WriteLine("ARE YOU ON THE LATEST VERSION?");
        Console.WriteLine($"YOU ARE RUNNING VERSION: {VERSION}.");
        Console.WriteLine($"THE LATEST VERSION ON GITHUB IS: {latestRelease.Name}");
        Console.WriteLine($"Do you want to continue? [y/n]");
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
                break;
            if (key.Key == ConsoleKey.N)
                return;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error checking latest version: {ex}");
}

Console.WriteLine($"SHARRandomizer.exe version: {VERSION}");
Console.WriteLine("Enter ip or port. If entry is just a port, then address will be assumed as archipelago.gg:");
string URI = Console.ReadLine();
if (int.TryParse(URI, out int porttest))
    URI = $"archipelago.gg:{URI}";

Console.WriteLine("Enter slot name:");
string SLOTNAME = Console.ReadLine();

Console.WriteLine("Enter password:");
string PASSWORD = Console.ReadLine();


ArchipelagoClient ac = new ArchipelagoClient();
ac.URI = URI;
ac.SLOTNAME = SLOTNAME;
ac.PASSWORD = PASSWORD;

MemoryManip mm = new MemoryManip();
mm.ac = ac;
InputListener im = new InputListener();

Thread connectThread = new Thread(ac.Connect);
connectThread.Start();

Task memoryTask = Task.Run(() => mm.MemoryStart());


memoryTask.Wait();

connectThread.Join();


