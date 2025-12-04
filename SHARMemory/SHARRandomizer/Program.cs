using Newtonsoft.Json;
using SHARRandomizer;
using SHARRandomizer.Classes;
using System.Diagnostics;

string VERSION = "Beta 0.4.0";

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
        Common.WriteLog("ARE YOU ON THE LATEST VERSION?", "GitHub");
        Common.WriteLog($"YOU ARE RUNNING VERSION: {VERSION}.", "GitHub");
        Common.WriteLog($"THE LATEST VERSION ON GITHUB IS: {latestRelease.Name}", "GitHub");
        Common.WriteLog($"Do you want to continue? [y/n]", "GitHub");
        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Y)
                break;

            if (key.Key == ConsoleKey.N)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/nmize1/AP-SHARRandomizer/releases/latest",
                    UseShellExecute = true
                });
                return;
            }
        }
    }
}
catch (Exception ex)
{
    Common.WriteLog($"Error checking latest version: {ex}", "GitHub");
}

Common.WriteLog($"SHARRandomizer.exe version: {VERSION}", "Main");
Common.WriteLog("Enter ip or port. If entry is just a port, then address will be assumed as archipelago.gg:", "Main");
string? URI;
do
    URI = Console.ReadLine();
while (string.IsNullOrWhiteSpace(URI));
if (int.TryParse(URI, out int porttest))
    URI = $"archipelago.gg:{URI}";

Common.WriteLog("Enter slot name:", "Main");
string? SLOTNAME;
do
    SLOTNAME = Console.ReadLine();
while (string.IsNullOrEmpty(SLOTNAME));

Common.WriteLog("Enter password:", "Main");
string PASSWORD = Console.ReadLine() ?? string.Empty;

ArchipelagoClient ac = new ArchipelagoClient();
ac.URI = URI;
ac.SLOTNAME = SLOTNAME;
ac.PASSWORD = PASSWORD;

MemoryManip mm = new MemoryManip(ac);
ac.mm = mm;

Thread connectThread = new Thread(ac.Connect);
connectThread.Start();

Task memoryTask = mm.MemoryStart();
await memoryTask;

connectThread.Join();


