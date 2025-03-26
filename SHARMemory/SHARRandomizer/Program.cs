using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Structs;
using SHARRandomizer;
using SHARRandomizer.Classes;
using System.Diagnostics;
using System.Drawing;

string VERSION = "Alpha 0.1.9";

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


