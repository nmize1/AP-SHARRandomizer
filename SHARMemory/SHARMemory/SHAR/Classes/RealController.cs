using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRealController@@")]
public class RealController : Class
{
    public const int NUM_CONTROLLER_TYPES = 4;
    public const int NUM_DIRECTION_TYPES = 4;

    public enum ControllerTypes
    {
        Gamepad,
        Keyboard,
        Mouse,
        SteeringWheel,
    }

    public enum DirectionType
    {
        Up,
        Down,
        Right,
        Left,
    }

    public RealController(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint RadControllerVFTableOffset = 0;

    internal const uint RadControllerOffset = RadControllerVFTableOffset + sizeof(uint);

    internal const uint ConnectedOffset = RadControllerOffset + sizeof(uint);
    public bool Connected
    {
        get => ReadBoolean(ConnectedOffset);
        set => WriteBoolean(ConnectedOffset, value);
    }

    internal const uint ControllerTypeOffset = ConnectedOffset + 4; // Padding
    public ControllerTypes ControllerType
    {
        get => (ControllerTypes)ReadUInt32(ControllerTypeOffset);
        set => WriteUInt32(ControllerTypeOffset, (uint)value);
    }

    internal const uint InputPointListOffset = ControllerTypeOffset + sizeof(uint);
    // typedef list< IRadControllerInputPoint* > RADINPUTPOINTLIST;
    // RADINPUTPOINTLIST   m_inputPointList;

    internal const uint InputToDICodeOffset = InputPointListOffset + 12;
    public StructArray<int> InputToDICode => new(Memory, ReadUInt32(InputToDICodeOffset), sizeof(int), NumInputPoints);

    internal const uint NumInputPointsOffset = InputToDICodeOffset + sizeof(uint);
    public int NumInputPoints
    {
        get => ReadInt32(NumInputPointsOffset);
        set => WriteInt32(NumInputPointsOffset, value);
    }

    public virtual void DisableButton(int mapType, int buttonId, DirectionType dir) { }

    public virtual void EnableButton(int mapType, int buttonId, DirectionType dir, InputManager.Buttons button) { }
}
