using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCarStartLocator@@")]
public class CarStartLocator : Locator
{
    public CarStartLocator(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint YRotationOffset = LocationOffset + Vector3.Size;
    public float YRotation
    {
        get => ReadSingle(YRotationOffset);
        set => WriteSingle(YRotationOffset, value);
    }
}
