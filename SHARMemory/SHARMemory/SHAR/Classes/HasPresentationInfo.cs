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

    internal const uint Bitfield_0x34Offset = AmbientNpcAnimationsOffset + 12;
    private byte Bitfield_0x34
    {
        get => ReadByte(Bitfield_0x34Offset);
        set => WriteByte(Bitfield_0x34Offset, value);
    }

    public bool PcIsChild
    {
        get => (Bitfield_0x34 & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield_0x34 |= 0b00000001;
            else
                Bitfield_0x34 &= 0b11111110;
        }
    }

    public bool NpcIsChild
    {
        get => (Bitfield_0x34 & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield_0x34 |= 0b00000010;
            else
                Bitfield_0x34 &= 0b11111101;
        }
    }

    public bool GoToPattyAndSelmaScreenWhenDone
    {
        get => (Bitfield_0x34 & 0b00000100) != 0;
        set
        {
            if (value)
                Bitfield_0x34 |= 0b00000100;
            else
                Bitfield_0x34 &= 0b11111011;
        }
    }

    internal const uint CamerasForLinesOfDialogOffset = Bitfield_0x34Offset + 4; // Padding
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mCamerasForLinesOfDialog;

    internal const uint BestSideLocatorOffset = CamerasForLinesOfDialogOffset + 12;
    public ulong BestSideLocator
    {
        get => ReadUInt64(BestSideLocatorOffset);
        set => WriteUInt64(BestSideLocatorOffset, value);
    }
}
