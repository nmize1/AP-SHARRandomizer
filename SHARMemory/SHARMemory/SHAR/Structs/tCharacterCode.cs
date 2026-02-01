using SHARMemory.Memory;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(tCharacterCodeStruct))]
public struct tCharacterCode
{
    public const int Size = sizeof(long) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public long NameHash;
    public uint CodePointer;
    public uint CodeHash;
    public uint NamePointer;
    public uint Unknown;

    public tCharacterCode(long nameHash, uint codePointer, uint codeHash, uint namePointer, uint unknown)
    {
        NameHash = nameHash;
        CodePointer = codePointer;
        CodeHash = codeHash;
        NamePointer = namePointer;
        Unknown = unknown;
    }

    public readonly string GetCode(Memory memory) => memory.ReadNullString(CodePointer, System.Text.Encoding.UTF8);
    public readonly string GetName(Memory memory) => memory.ReadNullString(NamePointer, System.Text.Encoding.UTF8);

    public override readonly string ToString() => $"<0x{NameHash:X2} | 0x{CodePointer:X2} | 0x{CodeHash:X2} | 0x{NamePointer:X2} | {Unknown}>";
}

internal class tCharacterCodeStruct : Struct
{
    public override int Size => tCharacterCode.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        long NameHash = BitConverter.ToInt64(Bytes, Offset);
        Offset += sizeof(long);
        uint CodePointer = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint CodeHash = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint NamePointer = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint Unknown = BitConverter.ToUInt32(Bytes, Offset);
        return new tCharacterCode(NameHash, CodePointer, CodeHash, NamePointer, Unknown);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not tCharacterCode Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(tCharacterCode)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.NameHash).CopyTo(Buffer, Offset);
        Offset += sizeof(long);
        BitConverter.GetBytes(Value2.CodePointer).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.CodeHash).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.NamePointer).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Unknown).CopyTo(Buffer, Offset);
    }
}
