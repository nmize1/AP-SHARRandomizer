using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Structs;
using SHARRandomizer;
using SHARRandomizer.Classes;
using System.Diagnostics;
using System.Drawing;
/*
LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
lt.PrintData();
return;
*/
ArchipelagoClient ac = new ArchipelagoClient();
MemoryManip mm = new MemoryManip();

Thread connectThread = new Thread(ac.Connect);
connectThread.Start();

Task memoryTask = Task.Run(() => mm.MemoryStart());


memoryTask.Wait();

connectThread.Join();


