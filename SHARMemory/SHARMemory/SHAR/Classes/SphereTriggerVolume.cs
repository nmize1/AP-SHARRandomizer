using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVSphereTriggerVolume@@")]
public class SphereTriggerVolume : TriggerVolume
{
    public SphereTriggerVolume(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint RadiusOffset = UserOffset + sizeof(int);
    public float Radius
    {
        get => ReadSingle(RadiusOffset);
        set => WriteSingle(RadiusOffset, value);
    }
}
