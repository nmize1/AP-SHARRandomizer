using SHARMemory.Memory;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(ShakeEventDataStruct))]
public struct ShakeEventData
{
    public const int Size = sizeof(int) + Vector3.Size + sizeof(float) + sizeof(bool);

    public int PlayerID;
    public Vector3 Direction;
    public float Force;
    public bool Looping;

    public ShakeEventData(int playerID, Vector3 direction, float force, bool looping)
    {
        PlayerID = playerID;
        Direction = direction;
        Force = force;
        Looping = looping;
    }

    public override readonly string ToString() => $"<{PlayerID} | {Direction} | {Force} | {Looping}>";
}

internal class ShakeEventDataStruct : Struct
{
    public override int Size => ShakeEventData.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        int PlayerID = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        Vector3 Direction = Memory.StructFromBytes<Vector3>(Bytes, Offset);
        Offset += Vector3.Size;
        float Force = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        bool Looping = BitConverter.ToBoolean(Bytes, Offset);
        return new ShakeEventData(PlayerID, Direction, Force, Looping);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not ShakeEventData Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(ShakeEventData)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.PlayerID).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        Memory.BytesFromStruct(Value2.Direction, Buffer, Offset);
        Offset += Vector3.Size;
        BitConverter.GetBytes(Value2.Force).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.Looping).CopyTo(Buffer, Offset);
    }
}
