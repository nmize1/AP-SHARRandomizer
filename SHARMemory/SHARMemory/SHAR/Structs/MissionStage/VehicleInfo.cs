using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Text;

namespace SHARMemory.SHAR.Structs.MissionStage;

[Struct(typeof(VehicleInfoStruct))]
public struct VehicleInfo
{
    public const int Size = sizeof(uint) + sizeof(uint) + sizeof(int) + sizeof(uint);

    public Vehicle Vehicle;

    public CarStartLocator Spawn;

    public int VehicleAINum;

    public VehicleAI VehicleAI;

    public VehicleInfo(Vehicle vehicle, CarStartLocator spawn, int vehicleAINum, VehicleAI vehicleAI)
    {
        Vehicle = vehicle;
        Spawn = spawn;
        VehicleAINum = vehicleAINum;
        VehicleAI = vehicleAI;
    }

    public override readonly string ToString() => $"{Vehicle} | {Spawn} | {VehicleAINum} | {VehicleAI}";
}

internal class VehicleInfoStruct : Struct
{
    public override int Size => VehicleInfo.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        Vehicle Vehicle = Memory.ClassFactory.Create<Vehicle>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        CarStartLocator Spawn = Memory.ClassFactory.Create<CarStartLocator>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        int VehicleAINum = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        VehicleAI VehicleAI = Memory.ClassFactory.Create<VehicleAI>(BitConverter.ToUInt32(Bytes, Offset));
        return new VehicleInfo(Vehicle, Spawn, VehicleAINum, VehicleAI);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not VehicleInfo Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(VehicleInfo)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Vehicle?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Spawn?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.VehicleAINum).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.VehicleAI?.Address ?? 0).CopyTo(Buffer, Offset);
    }
}
