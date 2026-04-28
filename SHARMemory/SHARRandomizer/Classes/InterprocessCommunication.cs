using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace SHARRandomizer.Classes;

public partial class InterprocessCommunication : IDisposable
{
    public enum InterprocessCommunicationType
    {
        GetVersion,
        IsHackLoaded,
        LookupString,
    }

    public enum DebugCommunicationType
    {
        CarCreateDriver,
        CarSetupHandling,
        CarTeleport,
        CharacterSetSkin,
        CharacterTeleport,
        RequestAsyncFileLoad,
        SimSetSetActive,
        ParseAndExecuteMFKorCON,
        CarRemoveFromWorld,
        DynaLoadDataSet,
        SkySetDrawable,
        GameModeSetCar,
        ChangeResolution,
        TriggerEvent,
    }

    public enum ModernResolutionSupportType
    {
        GetResolution
    }

    public enum DebugTestType
    {
        CamPreview,
        CamRestore,
    }


    [LibraryImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial SafePipeHandle CreateNamedPipeClient(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    [LibraryImport("kernel32.dll", EntryPoint = "WaitNamedPipeW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool WaitNamedPipe(string name, int timeout);

    private const int GENERIC_READ = unchecked((int)0x80000000);
    private const int GENERIC_WRITE = 0x40000000;
    private const int ERROR_FILE_NOT_FOUND = 2;
    private const int ERROR_PIPE_BUSY = 231;

    private readonly int ProcessId;
    public NamedPipeClientStream Stream;
    public BinaryWriter Writer;
    public BinaryReader Reader;

    public InterprocessCommunication(int processId)
    {
        ProcessId = processId;
        string path = @"\\.\pipe\LucasSimpsonsHitAndRunModLauncher" + processId;

        SafePipeHandle handle;

        while (true)
        {
            handle = CreateNamedPipeClient(path, GENERIC_READ | GENERIC_WRITE, FileShare.None, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();

                if (error == ERROR_FILE_NOT_FOUND)
                {
                    throw new Exception("Failed to create IPC Pipe: The Simpsons: Hit & Run must be launched with Lucas' Simpsons Hit & Run Mod Launcher version 1.17.1 or later, with the Interprocess Communication hack loaded.");
                }
                else if (error == ERROR_PIPE_BUSY)
                {
                    WaitNamedPipe(path, Timeout.Infinite);
                    continue;
                }
                else
                {
                    throw new Win32Exception(error);
                }
            }

            break;
        }

        Stream = new NamedPipeClientStream(PipeDirection.InOut, false, true, handle);
        Writer = new BinaryWriter(Stream);
        Reader = new BinaryReader(Stream);
    }

    public void Send(string hack)
    {
        Writer.Write((uint)hack.Length);
        Writer.Write(Encoding.Unicode.GetBytes(hack));

        uint result = Reader.ReadUInt32();

        if (result == 1)
            return;

        if (result == 0)
            throw new Exception($"This feature requires the {hack} hack to be loaded.");

        throw new Exception($"An unknown error occured: {result}");
    }

    public Version GetVersion()
    {
        Send("InterprocessCommunication");
        Writer.Write((uint)InterprocessCommunicationType.GetVersion);

        string version = ReadString(true);
        return new Version(version);
    }

    public void CheckVersion(string version)
    {
        var version2 = GetVersion();

        if (version == "1.17.1")
            return;

        if (version2.CompareTo(new Version(version)) < 0)
            throw new Exception($"This feature requires The Simspsons: Hit & Run to have been launched with version {version} or later of Lucas' Simpsons Hit & Run Mod Launcher.");
    }

    public bool IsHackLoaded(string hack)
    {
        Send("InterprocessCommunication");
        Writer.Write((uint)InterprocessCommunicationType.IsHackLoaded);

        WriteString(hack, true);

        byte result = Reader.ReadByte();
        return result != 0;
    }

    public void CheckHack(string hack)
    {
        if (!IsHackLoaded(hack))
            throw new Exception($"This freature requires the {hack} to be loaded.");
    }

    public void CheckVersionAndHack(string version, string hack)
    {
        if (version != "1.17.1")
        {
            var version2 = GetVersion();

            if (version2.CompareTo(new Version(version)) < 0)
                throw new Exception($"This feature requires The Simspsons: Hit & Run to have been launched with version {version} or later of Lucas' Simpsons Hit & Run Mod Launcher.");
        }

        CheckHack(hack);
    }

    public string? GetString(string name)
    {
        Send("InterprocessCommunication");
        Writer.Write((uint)InterprocessCommunicationType.LookupString);

        WriteString(name, false);

        byte result = Reader.ReadByte();

        if (result == 0)
            return null;

        return ReadString(true);
    }

    public string ReadString(bool unicode)
    {
        uint length = Reader.ReadUInt32();

        return unicode ? Encoding.Unicode.GetString(Reader.ReadBytes((int)length * 2)) : Encoding.ASCII.GetString(Reader.ReadBytes((int)length));
    }

    public void WriteString(string str, bool unicode)
    {
        Writer.Write((uint)str.Length);

        Writer.Write(unicode ? Encoding.Unicode.GetBytes(str) : Encoding.ASCII.GetBytes(str));
    }

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                Stream?.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}