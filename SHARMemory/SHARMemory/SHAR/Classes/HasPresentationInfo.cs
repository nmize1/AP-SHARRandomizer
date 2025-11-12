using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVHasPresentationInfo@@")]
public class HasPresentationInfo : Class
{
    public HasPresentationInfo(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ConversationCamNameOffset = 0;
    public ulong ConversationCamName
    {
        get => ReadUInt64(ConversationCamNameOffset);
        set => WriteUInt64(ConversationCamNameOffset, value);
    }

    internal const uint ConversationCamNpcNameOffset = ConversationCamNameOffset + sizeof(ulong);
    public ulong ConversationCamNpcName
    {
        get => ReadUInt64(ConversationCamNpcNameOffset);
        set => WriteUInt64(ConversationCamNpcNameOffset, value);
    }

    internal const uint ConversationCamPcNameOffset = ConversationCamNpcNameOffset + sizeof(ulong);
    public ulong ConversationCamPcName
    {
        get => ReadUInt64(ConversationCamPcNameOffset);
        set => WriteUInt64(ConversationCamPcNameOffset, value);
    }

    internal const uint AmbientPcAnimationsRandomOffset = ConversationCamPcNameOffset + sizeof(ulong);
    public bool AmbientPcAnimationsRandom
    {
        get => ReadBoolean(AmbientPcAnimationsRandomOffset);
        set => WriteBoolean(AmbientPcAnimationsRandomOffset, value);
    }

    internal const uint AmbientNpcAnimationsRandomOffset = AmbientPcAnimationsRandomOffset + 1;
    public bool AmbientNpcAnimationsRandom
    {
        get => ReadBoolean(AmbientNpcAnimationsRandomOffset);
        set => WriteBoolean(AmbientNpcAnimationsRandomOffset, value);
    }

    internal const uint AmbientPcAnimationsOffset = AmbientNpcAnimationsRandomOffset + 3; // Padding
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mAmbientPcAnimations;

    internal const uint AmbientNpcAnimationsOffset = AmbientPcAnimationsOffset + 12;
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mAmbientNpcAnimations;

    internal const uint PcIsChildOffset = AmbientNpcAnimationsOffset + 12;
    public bool PcIsChild
    {
        get => ReadBitfield(PcIsChildOffset, 0);
        set => WriteBitfield(PcIsChildOffset, 0, value);
    }

    internal const uint NpcIsChildOffset = PcIsChildOffset + 0;
    public bool NpcIsChild
    {
        get => ReadBitfield(NpcIsChildOffset, 1);
        set => WriteBitfield(NpcIsChildOffset, 1, value);
    }

    internal const uint GoToPattyAndSelmaScreenWhenDoneOffset = NpcIsChildOffset + 0;
    public bool GoToPattyAndSelmaScreenWhenDone
    {
        get => ReadBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2);
        set => WriteBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2, value);
    }

    internal const uint CamerasForLinesOfDialogOffset = GoToPattyAndSelmaScreenWhenDoneOffset + 4; // Padding
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mCamerasForLinesOfDialog;

    internal const uint BestSideLocatorOffset = CamerasForLinesOfDialogOffset + 12;
    public ulong BestSideLocator
    {
        get => ReadUInt64(BestSideLocatorOffset);
        set => WriteUInt64(BestSideLocatorOffset, value);
    }
}
