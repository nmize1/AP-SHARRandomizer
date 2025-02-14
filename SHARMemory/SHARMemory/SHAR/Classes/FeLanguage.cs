using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;
using System.Text;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVFeLanguage@@")]
public class FeLanguage : Class
{
    public FeLanguage(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public char LanguageID => (char)ReadByte(16);

    public uint Modulo => ReadUInt32(20);

    public uint BufferSize => ReadUInt32(24);

    public uint NumStrings => ReadUInt32(28);

    public StructArray<uint> Hashes => new(Memory, ReadUInt32(32), sizeof(uint), (int)NumStrings);

    public StructArray<uint> Offsets => new(Memory, ReadUInt32(36), sizeof(uint), (int)NumStrings);

    public byte[] Buffer
    {
        get => Memory.ReadBytes(ReadUInt32(40), BufferSize);
        set
        {
            if (value.Length != BufferSize)
                throw new ArgumentException($"{nameof(value)} must be of equal length to \"BufferSize\" ({BufferSize}).", nameof(value));

            Memory.WriteBytes(ReadUInt32(40), value);
        }
    }

    public int? GetIndex(uint hash)
    {
        var hashes = Hashes.ToArray();
        for (int i = 0; i < NumStrings; i++)
            if (hashes[i] == hash)
                return i;

        return null;
    }

    public int? GetIndex(string name) => GetIndex(GetHash(name));

    public string GetString(uint hash)
    {
        int? index = GetIndex(hash);
        if (!index.HasValue)
            return null;

        byte[] buffer = Buffer;

        int startPos = (int)Offsets[index.Value];
        int endPos = startPos;
        while (endPos < buffer.Length && buffer[endPos] != 0)
            endPos += 2;

        return ProcessMemory.NullTerminate(Encoding.Unicode.GetString(Buffer, startPos, endPos - startPos));
    }

    public string GetString(string name) => GetString(GetHash(name));

    public bool SetString(uint hash, string value)
    {
        if (value.Length == 0)
            throw new ArgumentException($"{nameof(value)} cannot be an empty string. Must have at least one char.");

        int? index = GetIndex(hash);
        if (!index.HasValue)
            return false;

        byte[] buffer = Buffer;

        int startPos = (int)Offsets[index.Value];
        int endPos = startPos;
        bool foundNull = false;
        while (endPos < buffer.Length)
        {
            endPos += 2;
            if (endPos >= buffer.Length)
            {
                endPos = buffer.Length;
                break;
            }
            if (buffer[endPos] == 0)
                foundNull = true;
            else if (foundNull)
                break;
        }

        int maxLength = endPos - startPos;

        byte[] bytes = Encoding.Unicode.GetBytes(value);
        if (bytes.Length > maxLength)
            throw new ArgumentException($"{nameof(value)} has a byte count of {bytes.Length}. Must be less than {maxLength}.", nameof(value));

        byte[] paddedBytes = new byte[maxLength];
        bytes.CopyTo(paddedBytes, 0);

        Memory.WriteBytes(ReadUInt32(40) + (uint)startPos, paddedBytes);

        return true;
    }

    public bool SetString(string name, string value) => SetString(GetHash(name), value);

    public uint GetHash(string name)
    {
        uint Hash = 0;

        var modulo = Modulo;
        foreach (char c in name)
            Hash = ((byte)c + (Hash << 6)) % modulo;

        return Hash;
    }
}
