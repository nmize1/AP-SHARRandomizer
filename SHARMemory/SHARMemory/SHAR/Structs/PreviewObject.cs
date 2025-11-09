using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Text;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(PreviewObjectStruct))]
public struct PreviewObject
{
    public const int Size = 16 + 64 + 16 + 64 + 4 + sizeof(uint);

    public string Name;

    public string Filename;

    public string NameModel;

    public string FilenameModel;

    public bool IsUnlocked;

    public Reward Reward;

    public PreviewObject(string name, string filename, string nameModel, string filenameModel, bool isUnlocked, Reward reward)
    {
        Name = name;
        Filename = filename;
        NameModel = nameModel;
        FilenameModel = filenameModel;
        IsUnlocked = isUnlocked;
        Reward = reward;
    }

    public override readonly string ToString() => $"{Name} | {Filename} | {NameModel} | {FilenameModel} | {IsUnlocked} | {Reward}";
}

internal class PreviewObjectStruct : Struct
{
    public override int Size => PreviewObject.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        string Name = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 16));
        Offset += 16;
        string Filename = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 64));
        Offset += 64;
        string NameModel = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 16));
        Offset += 16;
        string FilenameModel = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 64));
        Offset += 64;
        bool IsUnlcoked = (Bytes[Offset] & 1) == 1;
        Offset += 4;
        Reward Reward = Memory.ClassFactory.Create<Reward>(BitConverter.ToUInt32(Bytes, Offset));
        return new PreviewObject(Name, Filename, NameModel, FilenameModel, IsUnlcoked, Reward);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not PreviewObject Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(PreviewObject)}'.", nameof(Value));

        Memory.GetStringBytes(Value2.Name, Encoding.UTF8, 16).CopyTo(Buffer, Offset);
        Offset += 16;
        Memory.GetStringBytes(Value2.Filename, Encoding.UTF8, 64).CopyTo(Buffer, Offset);
        Offset += 64;
        Memory.GetStringBytes(Value2.NameModel, Encoding.UTF8, 16).CopyTo(Buffer, Offset);
        Offset += 16;
        Memory.GetStringBytes(Value2.FilenameModel, Encoding.UTF8, 64).CopyTo(Buffer, Offset);
        Offset += 64;
        BitConverter.GetBytes(Value2.IsUnlocked).CopyTo(Buffer, Offset);
        Offset += 4;
        BitConverter.GetBytes(Value2.Reward?.Address ?? 0).CopyTo(Buffer, Offset);
    }
}
