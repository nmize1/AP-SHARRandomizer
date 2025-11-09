using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVConversation@@")]
public class Conversation : PlayableDialog
{
    public Conversation(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint MaxOrderNumberOffset = NextListObjectOffset + sizeof(uint);
    public uint MaxOrderNumber
    {
        get => ReadUInt32(MaxOrderNumberOffset);
        set => WriteUInt32(MaxOrderNumberOffset, value);
    }

    internal const uint DialogListOffset = MaxOrderNumberOffset + sizeof(uint);
    public DialogLine DialogList => Memory.ClassFactory.Create<DialogLine>(ReadUInt32(DialogListOffset));

    internal const uint CurrentLineOffset = DialogListOffset + sizeof(uint);
    public DialogLine CurrentLine => Memory.ClassFactory.Create<DialogLine>(ReadUInt32(CurrentLineOffset));
}
