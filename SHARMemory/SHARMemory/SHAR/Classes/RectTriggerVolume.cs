using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRectTriggerVolume@@")]
public class RectTriggerVolume : TriggerVolume
{
    public RectTriggerVolume(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint AxisXOffset = UserOffset + sizeof(int);
    public Vector3 AxisX
    {
        get => ReadStruct<Vector3>(AxisXOffset);
        set => WriteStruct(AxisXOffset, value);
    }

    internal const uint AxisYOffset = AxisXOffset + Vector3.Size;
    public Vector3 AxisY
    {
        get => ReadStruct<Vector3>(AxisYOffset);
        set => WriteStruct(AxisYOffset, value);
    }

    internal const uint AxisZOffset = AxisYOffset + Vector3.Size;
    public Vector3 AxisZ
    {
        get => ReadStruct<Vector3>(AxisZOffset);
        set => WriteStruct(AxisZOffset, value);
    }
    
    internal const uint ExtentXOffset = AxisZOffset + Vector3.Size;
    public float ExtentX
    {
        get => ReadSingle(ExtentXOffset);
        set => WriteSingle(ExtentXOffset, value);
    }

    internal const uint ExtentYOffset = ExtentXOffset + sizeof(float);
    public float ExtentY
    {
        get => ReadSingle(ExtentYOffset);
        set => WriteSingle(ExtentYOffset, value);
    }

    internal const uint ExtentZOffset = ExtentYOffset + sizeof(float);
    public float ExtentZ
    {
        get => ReadSingle(ExtentZOffset);
        set => WriteSingle(ExtentZOffset, value);
    }

    internal const uint RadiusOffset = ExtentZOffset + sizeof(float);
    public float Radius
    {
        get => ReadSingle(RadiusOffset);
        set => WriteSingle(RadiusOffset, value);
    }

    internal const uint World2TriggerOffset = RadiusOffset + sizeof(float);
    public Matrix4x4 World2Trigger
    {
        get => ReadStruct<Matrix4x4>(World2TriggerOffset);
        set => WriteStruct(World2TriggerOffset, value);
    }

    internal const uint Trigger2WorldOffset = World2TriggerOffset + Matrix4x4.Size;
    public Matrix4x4 Trigger2World
    {
        get => ReadStruct<Matrix4x4>(Trigger2WorldOffset);
        set => WriteStruct(Trigger2WorldOffset, value);
    }
}
