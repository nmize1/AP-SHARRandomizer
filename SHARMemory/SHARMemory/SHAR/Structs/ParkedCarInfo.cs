using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Text;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(ParkedCarInfoStruct))]
public struct ParkedCarInfo
{
    public const int Size = 33 + 3 + sizeof(uint) + sizeof(uint) + sizeof(long);

    public string Name;
    public Vehicle Car;
    public Vehicle Husk;
    public long LoadedZoneUID;

    public ParkedCarInfo(string name, Vehicle car, Vehicle husk, long loadedZoneUID)
    {
        Name = name;
        Car = car;
        Husk = husk;
        LoadedZoneUID = loadedZoneUID;
    }

    public override readonly string ToString() => $"{Name} | {Car} | {Husk} | {LoadedZoneUID}";
}

internal class ParkedCarInfoStruct : Struct
{
    public override int Size => ParkedCarInfo.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        string Name = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 33));
        Offset += 33 + 3;
        Vehicle Car = Memory.ClassFactory.Create<Vehicle>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Vehicle Husk = Memory.ClassFactory.Create<Vehicle>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        long LoadedZoneUID = BitConverter.ToInt64(Bytes, Offset);
        return new ParkedCarInfo(Name, Car, Husk, LoadedZoneUID);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not ParkedCarInfo Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(ParkedCarInfo)}'.", nameof(Value));

        Memory.GetStringBytes(Value2.Name, Encoding.UTF8, 33).CopyTo(Buffer, Offset);
        Offset += 33 + 3;
        BitConverter.GetBytes(Value2.Car?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Husk?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.LoadedZoneUID).CopyTo(Buffer, Offset);
    }
}
