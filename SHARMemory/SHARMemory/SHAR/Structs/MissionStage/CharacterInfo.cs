using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Text;

namespace SHARMemory.SHAR.Structs.MissionStage;

[Struct(typeof(CharacterInfoStruct))]
public struct CharacterInfo
{
    public const int Size = 16 + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + 16;

    public string Name;

    public Character Character;

    public Locator Locator;

    public Locator CarLocator;

    public ZoneEventLocator ZoneEventLocator;

    public Vehicle Vehicle;

    public string VehicleName;

    public CharacterInfo(string name, Character character, Locator locator, Locator carLocator, ZoneEventLocator zoneEventLocator, Vehicle vehicle, string vehicleName)
    {
        Name = name;
        Character = character;
        Locator = locator;
        CarLocator = carLocator;
        ZoneEventLocator = zoneEventLocator;
        Vehicle = vehicle;
        VehicleName = vehicleName;
    }

    public override readonly string ToString() => $"{Name} | {Character} | {Locator} | {CarLocator} | {ZoneEventLocator} | {Vehicle} | {VehicleName}";
}

internal class CharacterInfoStruct : Struct
{
    public override int Size => CharacterInfo.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        string Name = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 16));
        Offset += 16;
        Character Character = Memory.ClassFactory.Create<Character>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Locator Locator = Memory.ClassFactory.Create<Locator>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Locator CarLocator = Memory.ClassFactory.Create<Locator>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        ZoneEventLocator ZoneEventLocator = Memory.ClassFactory.Create<ZoneEventLocator>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Vehicle Vehicle = Memory.ClassFactory.Create<Vehicle>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        string VehicleName = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 16));
        return new CharacterInfo(Name, Character, Locator, CarLocator, ZoneEventLocator, Vehicle, VehicleName);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not CharacterInfo Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(CharacterInfo)}'.", nameof(Value));

        Memory.GetStringBytes(Value2.Name, Encoding.UTF8, 16).CopyTo(Buffer, Offset);
        Offset += 16;
        BitConverter.GetBytes(Value2.Character?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Locator?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.CarLocator?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.ZoneEventLocator?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Vehicle?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        Memory.GetStringBytes(Value2.VehicleName, Encoding.UTF8, 16).CopyTo(Buffer, Offset);
    }
}
