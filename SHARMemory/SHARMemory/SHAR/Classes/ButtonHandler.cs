using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVButtonHandler@ActionButton@@")]
public class ButtonHandler : tRefCounted
{
    public ButtonHandler(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ActionButtonOffset = RefCountOffset + sizeof(uint);
    public CharacterController.Intentions ActionButton
    {
        get => (CharacterController.Intentions)ReadUInt32(ActionButtonOffset);
        set => WriteUInt32(ActionButtonOffset, (uint)value);
    }
}
