using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SHARMemory.Memory;

namespace SHARMemory.SHAR;

/// <summary>
/// Class <c>SHAR.Memory</c> inherits <c>ProcessMemory</c>, and adds additional SHAR-specific methods and properties.
/// Initially created by Lucas Cardellini, further developed by Proddy.
/// </summary>
public class Memory : ProcessMemory
{
    /// <summary>
    /// An enum of supported game versions.
    /// </summary>
    public enum GameVersions
    {
        /// <summary>
        /// The English release of the game.
        /// </summary>
        ReleaseEnglish,
        /// <summary>
        /// The International release of the game.
        /// </summary>
        ReleaseInternational,
        /// <summary>
        /// The Best Seller release of the game.
        /// </summary>
        BestSellerSeries,
        /// <summary>
        /// The Demo release of the game.
        /// </summary>
        Demo,
        /// <summary>
        /// An Unknown release of the game.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// An enum of game sub versions - <see cref="GameVersions.ReleaseInternational"/>
    /// </summary>
    public enum GameSubVersions
    {
        /// <summary>
        /// The English language release of <see cref="GameVersions.ReleaseInternational"/>.
        /// </summary>
        English,
        /// <summary>
        /// The French language release of <see cref="GameVersions.ReleaseInternational"/>.
        /// </summary>
        French,
        /// <summary>
        /// The German language release of <see cref="GameVersions.ReleaseInternational"/>.
        /// </summary>
        German,
        /// <summary>
        /// The Spanish language release of <see cref="GameVersions.ReleaseInternational"/>.
        /// </summary>
        Spanish,
        /// <summary>
        /// An Unknown release of <see cref="GameVersions.ReleaseInternational"/>.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Maps <see href="https://modbakery.donutteam.com/releases/view/lucas-mod-launcher" langword=" (Lucas' Mod Launcher)" /> <c>TypeInfoName</c>s to the SHAR <c>TypeInfoName</c>s.
    /// </summary>
    private static readonly Dictionary<string, string> TypeInfoNames = new()
    {
        { ".?AVCD3DShader@@", ".?AVd3dShader@@" },
        { ".?AVCPDDIBaseShader@@", ".?AVpddiBaseShader@@" },
        { ".?AVCPDDIShader@@", ".?AVpddiShader@@" },
        { ".?AVCPDDIObject@@", ".?AVpddiObject@@" },
        // TODO: Add other TypeInfoNames
    };

    private static readonly Dictionary<string, Type> BuiltInClasses = ClassFactory.GetClasses(System.Reflection.Assembly.GetExecutingAssembly(), "SHARMemory.SHAR.Classes");

    /// <summary>
    /// The detected <see cref="GameVersions"/>
    /// </summary>
    public GameVersions GameVersion { get; }
    /// <summary>
    /// The detected <see cref="GameSubVersions"/>
    /// </summary>
    public GameSubVersions GameSubVersion { get; }

    /// <summary>
    /// References to SHAR's static globals.
    /// </summary>
    public readonly Globals Globals;
    /// <summary>
    /// References to SHAR's singletons.
    /// </summary>
    public readonly Singletons Singletons;
    /// <summary>
    /// References to SHAR's custom functions.
    /// </summary>
    public readonly Functions Functions;
    /// <summary>
    /// The watcher class for monitoring game changes in realtime.
    /// </summary>
    public readonly Watcher Watcher;

    /// <summary>
    /// Checks if <see href="https://modbakery.donutteam.com/releases/view/lucas-mod-launcher" langword=" (Lucas' Mod Launcher)" /> is loaded.
    /// </summary>
    public bool IsModLauncherLoaded { get; }

    private static readonly uint[] ModLauncherOrdinalKeys =
    [
        3151, // Event Hacks

        3360, // Max Cars
        3364, // Cars Offset

        3362, // Max Traffic

        3947, // Max Stages
        3948, // Stages Offset
    ];
    /// <summary>
    /// A <c>Dictionary</c> containing a list of ordinals used by <see href="https://modbakery.donutteam.com/releases/view/lucas-mod-launcher" langword=" (Lucas' Mod Launcher)" /> and their addresses.
    /// </summary>
    internal Dictionary<uint, uint> ModLauncherOrdinals { get; } = new Dictionary<uint, uint>(ModLauncherOrdinalKeys.Length);
    private void LoadModLauncherOrdinals(ProcessModule hacksModule)
    {
        IntPtr dll = LoadLibraryEx(hacksModule.FileName, IntPtr.Zero, LoadLibraryFlags.DONT_RESOLVE_DLL_REFERENCES);
        if (dll == IntPtr.Zero)
        {
            Debug.WriteLine($"Couldn't load module: {hacksModule.FileName}");
            return;
        }

        try
        {
            foreach (uint ordinal in ModLauncherOrdinalKeys)
            {
                UIntPtr method = GetProcAddressOrdinal(dll, new UIntPtr(ordinal));
                if (method == UIntPtr.Zero)
                {
                    Debug.WriteLine($"Couldn't find method: {ordinal}");
                    continue;
                }

                uint offset = (uint)(method.ToUInt32() - dll.ToInt32());

                uint address = (uint)(hacksModule.BaseAddress.ToInt32() + offset);

                ModLauncherOrdinals.Add(ordinal, address);
            }
        }
        finally
        {
            FreeLibrary(dll);
        }
    }

    /// <summary>
    /// If <see href="https://modbakery.donutteam.com/releases/view/lucas-mod-launcher" langword=" (Lucas' Mod Launcher)" /> is loaded, query loaded hacks.
    /// Credits: Lucas Cardellini
    /// </summary>
    /// <returns>
    /// A string array containing the names of loaded hacks.
    /// </returns>
    public string[] GetLoadedHacks()
    {
        if (!ModLauncherOrdinals.TryGetValue(3351, out uint EventHacks))
            return null;

        uint HacksLoaded = ReadUInt32(EventHacks + 8);

        string[] Hacks = new string[HacksLoaded];

        if (HacksLoaded > 0)
        {
            int i = 0;
            uint node = ReadUInt32(EventHacks);
            while (node != 0)
            {
                uint hack = ReadUInt32(node + 12);
                Hacks[i] = ReadString(hack + 562, Encoding.Unicode, 128u);
                i++;

                node = ReadUInt32(node + 8);
            }
        }

        return Hacks;
    }

    /// <summary>
    /// If <see href="https://modbakery.donutteam.com/releases/view/lucas-mod-launcher" langword=" (Lucas' Mod Launcher)" /> is loaded, check if a hack is loaded.
    /// Credits: Lucas Cardellini
    /// </summary>
    /// <param name="HackName">
    /// The name of the hack to check for.
    /// </param>
    /// <returns>
    /// A boolean if reporting if the specified hack is found to be loaded.
    /// </returns>
    public bool IsHackLoaded(string HackName)
    {
        if (!ModLauncherOrdinals.TryGetValue(3351, out uint EventHacks))
            return false;

        uint node = ReadUInt32(EventHacks);
        while (node != 0)
        {
            uint hack = ReadUInt32(node + 12);
            if (ReadString(hack + 562, Encoding.Unicode, 128u).Equals(HackName, StringComparison.OrdinalIgnoreCase))
                return true;

            node = ReadUInt32(node + 8);
        }

        return false;
    }

    /// <summary>
    /// Checks a somewhat arbitrary memory value to try determine what version of the game is running.
    /// Credits: Lucas Cardellini
    /// </summary>
    /// <param name="GameSubVersion">
    /// If the return value is <c>GameVersions.ReleaseInternational</c>, which sub version of the International release it is.
    /// </param>
    /// <returns>
    /// One of the <c>GameVersions</c> enum matching which version was detected.
    /// </returns>
    private GameVersions DetectVersion(ref GameSubVersions GameSubVersion)
    {
        switch (ReadUInt32(0x593FFF))
        {
            case 0xFAE804C5:
                return GameVersions.Demo;
            case 0x4B8B2274:
                return GameVersions.ReleaseEnglish;
            case 0xC985ED33:
                GameSubVersion = ReadUInt32(0x494FB1) switch
                {
                    0xF6C10AE8 => GameSubVersions.English,
                    0xF984BAE8 => GameSubVersions.French,
                    0xF6C12AE8 => GameSubVersions.German,
                    0xF6C0FAE8 => GameSubVersions.Spanish,
                    _ => GameSubVersions.Unknown,
                };
                return GameVersions.ReleaseInternational;
            case 0xFC468D05:
                return GameVersions.BestSellerSeries;
            default:
                return GameVersions.Unknown;
        }
    }

    /// <summary>
    /// Based on the detected game version, returns the matching address.
    /// Credits: Lucas Cardellini
    /// </summary>
    /// <param name="ReleaseEnglishAddress">
    /// The address of the English release.
    /// </param>
    /// <param name="DemoAddress">
    /// The address of the Demo release.
    /// </param>
    /// <param name="ReleaseInternationalAddress">
    /// The address of the International release.
    /// </param>
    /// <param name="BestSellerSeriesAddress">
    /// The address of the Best Seller release.
    /// </param>
    /// <returns>
    /// The given <c>uint</c> that matches the game version.
    /// </returns>
    /// <exception cref="Exception">
    /// Throws an error in the detected game version is unknown.
    /// </exception>
    public uint SelectAddress(uint ReleaseEnglishAddress, uint DemoAddress, uint ReleaseInternationalAddress, uint BestSellerSeriesAddress)
    {
        var address = GameVersion switch
        {
            GameVersions.ReleaseEnglish => ReleaseEnglishAddress,
            GameVersions.Demo => DemoAddress,
            GameVersions.ReleaseInternational => ReleaseInternationalAddress,
            GameVersions.BestSellerSeries => BestSellerSeriesAddress,
            _ => throw new Exception("Unrecognised game version."),
        };
        if (address == 0)
            throw new NotImplementedException($"This address is not supported on {GameVersion}.");
        return address;
    }

    /// <summary>
    /// The <c>SHAR.Memory</c> constructor.
    /// </summary>
    /// <param name="Process">
    /// A <c>Process</c> that points to a SHAR instance. See: <see cref="GetSHARProcess"/>.
    /// </param>
    public Memory(Process Process) : base(Process)
    {
        ClassFactory.AddTypeInfoNames(TypeInfoNames);
        ClassFactory.AddClasses(BuiltInClasses);

        GameSubVersions subVersion = GameSubVersions.English;
        GameVersion = DetectVersion(ref subVersion);
        GameSubVersion = subVersion;

        using (ProcessModule hacksModule = Process.Modules.Cast<ProcessModule>().FirstOrDefault(x => x.ModuleName.Equals("Hacks.dll", StringComparison.OrdinalIgnoreCase)))
        {
            if (hacksModule == null)
            {
                IsModLauncherLoaded = false;
            }
            else
            {
                IsModLauncherLoaded = true;
                LoadModLauncherOrdinals(hacksModule);
            }
        }

        Globals = new(this);
        Singletons = new(this);
        Functions = new(this);
        Watcher = new(this);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private static IntPtr GetGameWindow() => FindWindow("The Simpsons Hit & Run", null);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int lpwdProcessId);

    /// <summary>
    /// Tries to get a SHAR <c>Process</c> based on window class.
    /// </summary>
    /// <returns>
    /// A SHAR <c>Process</c> or <c>null</c>.
    /// </returns>
    public static Process GetSHARProcess()
    {
        IntPtr GameWindow = GetGameWindow();
        if (GameWindow == IntPtr.Zero)
            return null;

        int ProcessId = 0;
        _ = GetWindowThreadProcessId(GameWindow, ref ProcessId);
        return Process.GetProcessById(ProcessId);
    }
}
