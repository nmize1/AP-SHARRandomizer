using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(TrafficHornQueueStruct))]
public struct TrafficHornQueue
{
    public const int Size = sizeof(uint) + sizeof(uint);

    public Vehicle Vehicle;

    public uint DelayInMilliseconds;

    public TrafficHornQueue(Vehicle vehicle, uint delayInMilliseconds)
    {
        Vehicle = vehicle;
        DelayInMilliseconds = delayInMilliseconds;
    }

    public override readonly string ToString() => $"{Vehicle} | {DelayInMilliseconds}";
}

internal class TrafficHornQueueStruct : Struct
{
    public override int Size => TrafficHornQueue.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        Vehicle vehicle = Memory.ClassFactory.Create<Vehicle>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        uint delayInMilliseconds = BitConverter.ToUInt32(Bytes, Offset);
        return new TrafficHornQueue(vehicle, delayInMilliseconds);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not TrafficHornQueue Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(TrafficHornQueue)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Vehicle?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.DelayInMilliseconds).CopyTo(Buffer, Offset);
    }
}
