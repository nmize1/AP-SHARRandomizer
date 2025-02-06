using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(DialogEventDataStruct))]
public struct DialogEventData
{
    public const int Size = sizeof(uint) + sizeof(uint) + sizeof(ulong) + sizeof(ulong) + sizeof(uint);

    public Character Char1;

    public Character Char2;

    public ulong CharUID1;

    public ulong CharUID2;

    public uint DialogName;

    public DialogEventData(Character char1, Character char2, ulong charUID1, ulong charUID2, uint dialogName)
    {
        Char1 = char1;
        Char2 = char2;
        CharUID1 = charUID1;
        CharUID2 = charUID2;
        DialogName = dialogName;
    }

    public override readonly string ToString() => $"{Char1} | {Char2} | {CharUID1} | {CharUID2} | {DialogName}";
}

internal class DialogEventDataStruct : Struct
{
    public override int Size => DialogEventData.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        Character char1 = Memory.ClassFactory.Create<Character>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Character char2 = Memory.ClassFactory.Create<Character>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        ulong charUID1 = BitConverter.ToUInt64(Bytes, Offset);
        Offset += sizeof(ulong);
        ulong charUID2 = BitConverter.ToUInt64(Bytes, Offset);
        Offset += sizeof(ulong);
        uint dialogName = BitConverter.ToUInt32(Bytes, Offset);
        return new DialogEventData(char1, char2, charUID1, charUID2, dialogName);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not DialogEventData Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(DialogEventData)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Char1?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Char2?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.CharUID1).CopyTo(Buffer, Offset);
        Offset += sizeof(ulong);
        BitConverter.GetBytes(Value2.CharUID2).CopyTo(Buffer, Offset);
        Offset += sizeof(ulong);
        BitConverter.GetBytes(Value2.DialogName).CopyTo(Buffer, Offset);
    }
}
