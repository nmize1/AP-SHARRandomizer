using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SHARMemory.Memory;

/// <summary>
/// Represents a parsed PE32 (32-bit) Portable Executable file.
/// This class allows reading export ordinals and RVAs without loading the DLL,
/// making it safe to use from a 64-bit process.
/// </summary>
public sealed class PeFile
{
    private const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;  // "MZ"
    private const uint IMAGE_NT_SIGNATURE = 0x00004550; // "PE\0\0"

    private const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10B; // PE32
    private const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20B; // PE32+

    /// <summary>
    /// Gets a mapping of exported ordinals to their corresponding RVAs.
    /// The RVA is relative to the module base at runtime.
    /// </summary>
    public IReadOnlyDictionary<uint, uint> ExportsByOrdinal => _exportsByOrdinal;

    /// <summary>
    /// Gets the preferred image base specified in the PE optional header.
    /// </summary>
    public uint ImageBase { get; private set; }

    private readonly Dictionary<uint, uint> _exportsByOrdinal = [];

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_DOS_HEADER
    {
        public ushort e_magic;
        public ushort e_cblp, e_cp, e_crlc, e_cparhdr;
        public ushort e_minalloc, e_maxalloc;
        public ushort e_ss, e_sp, e_csum;
        public ushort e_ip, e_cs;
        public ushort e_lfarlc, e_ovno;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] e_res1;
        public ushort e_oemid, e_oeminfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] e_res2;
        public int e_lfanew;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_OPTIONAL_HEADER32
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;
        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_NT_HEADERS32
    {
        public uint Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_SECTION_HEADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_EXPORT_DIRECTORY
    {
        public uint Characteristics;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Name;
        public uint Base;
        public uint NumberOfFunctions;
        public uint NumberOfNames;
        public uint AddressOfFunctions;
        public uint AddressOfNames;
        public uint AddressOfNameOrdinals;
    }

    /// <summary>
    /// Loads and parses a PE32 (32-bit) executable or DLL from disk.
    /// The file is not loaded into memory as an executable image.
    /// </summary>
    /// <param name="path">Path to the PE file.</param>
    /// <exception cref="InvalidDataException">
    /// Thrown if the file is not a valid PE32 image.
    /// </exception>
    public PeFile(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        var dos = ReadStruct<IMAGE_DOS_HEADER>(br);
        if (dos.e_magic != IMAGE_DOS_SIGNATURE)
            throw new InvalidDataException("Invalid DOS header");

        fs.Position = dos.e_lfanew;

        var nt = ReadStruct<IMAGE_NT_HEADERS32>(br);
        if (nt.Signature != IMAGE_NT_SIGNATURE || nt.OptionalHeader.Magic != IMAGE_NT_OPTIONAL_HDR32_MAGIC)
            throw new InvalidDataException("Not a PE32 file");

        ImageBase = nt.OptionalHeader.ImageBase;

        var sections = new IMAGE_SECTION_HEADER[nt.FileHeader.NumberOfSections];
        for (var i = 0; i < sections.Length; i++)
            sections[i] = ReadStruct<IMAGE_SECTION_HEADER>(br);

        var exportDir = nt.OptionalHeader.DataDirectory[0];
        if (exportDir.VirtualAddress == 0)
            return;

        var exportOffset = RvaToFileOffset(exportDir.VirtualAddress, sections);
        fs.Position = exportOffset;

        var exports = ReadStruct<IMAGE_EXPORT_DIRECTORY>(br);

        fs.Position = RvaToFileOffset(exports.AddressOfFunctions, sections);

        var functionRVAs = new uint[exports.NumberOfFunctions];
        for (var i = 0; i < functionRVAs.Length; i++)
            functionRVAs[i] = br.ReadUInt32();

        for (var i = 0u; i < exports.NumberOfFunctions; i++)
        {
            var ordinal = exports.Base + i;
            var rva = functionRVAs[i];

            if (rva != 0)
                _exportsByOrdinal[ordinal] = rva;
        }
    }

    private static T ReadStruct<T>(BinaryReader br) where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        var data = br.ReadBytes(size);

        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        }
        finally
        {
            handle.Free();
        }
    }

    private static uint RvaToFileOffset(uint rva, IMAGE_SECTION_HEADER[] sections)
    {
        foreach (var s in sections)
        {
            var size = Math.Max(s.VirtualSize, s.SizeOfRawData);
            if (rva >= s.VirtualAddress && rva < s.VirtualAddress + size)
                return rva - s.VirtualAddress + s.PointerToRawData;
        }
        throw new InvalidOperationException($"Invalid RVA 0x{rva:X}");
    }

    /// <summary>
    /// Computes the absolute runtime address of an exported function
    /// given the module base address in memory.
    /// </summary>
    /// <param name="moduleBase">Base address where the module is loaded.</param>
    /// <param name="ordinal">Export ordinal.</param>
    /// <returns>The absolute runtime address.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the ordinal is not exported.
    /// </exception>
    public uint GetRuntimeAddress(IntPtr moduleBase, uint ordinal)
    {
        if (!_exportsByOrdinal.TryGetValue(ordinal, out var rva))
            throw new KeyNotFoundException($"Ordinal {ordinal} not exported");

        return (uint)moduleBase.ToInt32() + rva;
    }
}
