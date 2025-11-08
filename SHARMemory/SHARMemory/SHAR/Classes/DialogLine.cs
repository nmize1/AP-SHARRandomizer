using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVDialogLine@@")]
public class DialogLine : PlayableDialog
{
    public enum Roles : byte
    {
        None,
        Walker,
        Driver,
        Pedestrian,
        Villain
    }

    public DialogLine(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ResourceOffset = NextListObjectOffset + sizeof(uint);

    internal const uint ConversationNameOffset = ResourceOffset + sizeof(uint);
    public uint ConversationName
    {
        get => ReadUInt32(ConversationNameOffset);
        set => WriteUInt32(ConversationNameOffset, value);
    }

    internal const uint RoleOffset = ConversationNameOffset + sizeof(uint);
    public Roles Role
    {
        get => (Roles)ReadByte(RoleOffset);
        set => WriteByte(RoleOffset, (byte)value);
    }

    internal const uint CharacterIndexOffset = RoleOffset + sizeof(byte);
    public byte CharacterIndex
    {
        get => ReadByte(CharacterIndexOffset);
        set => WriteByte(CharacterIndexOffset, value);
    }
}
