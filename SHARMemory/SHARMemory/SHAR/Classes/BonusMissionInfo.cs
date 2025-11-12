using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Text;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVBonusMissionInfo@@")]
public class BonusMissionInfo : HasPresentationInfo
{
    public BonusMissionInfo(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint BonusMissionInfoVFTableOffset = 0;

    internal new const uint ConversationCamNameOffset = BonusMissionInfoVFTableOffset + sizeof(uint);
    public new ulong ConversationCamName
    {
        get => ReadUInt64(ConversationCamNameOffset);
        set => WriteUInt64(ConversationCamNameOffset, value);
    }

    internal new const uint ConversationCamNpcNameOffset = ConversationCamNameOffset + sizeof(ulong);
    public new ulong ConversationCamNpcName
    {
        get => ReadUInt64(ConversationCamNpcNameOffset);
        set => WriteUInt64(ConversationCamNpcNameOffset, value);
    }

    internal new const uint ConversationCamPcNameOffset = ConversationCamNpcNameOffset + sizeof(ulong);
    public new ulong ConversationCamPcName
    {
        get => ReadUInt64(ConversationCamPcNameOffset);
        set => WriteUInt64(ConversationCamPcNameOffset, value);
    }

    internal new const uint AmbientPcAnimationsRandomOffset = ConversationCamPcNameOffset + sizeof(ulong);
    public new bool AmbientPcAnimationsRandom
    {
        get => ReadBoolean(AmbientPcAnimationsRandomOffset);
        set => WriteBoolean(AmbientPcAnimationsRandomOffset, value);
    }

    internal new const uint AmbientNpcAnimationsRandomOffset = AmbientPcAnimationsRandomOffset + 1;
    public new bool AmbientNpcAnimationsRandom
    {
        get => ReadBoolean(AmbientNpcAnimationsRandomOffset);
        set => WriteBoolean(AmbientNpcAnimationsRandomOffset, value);
    }

    internal new const uint AmbientPcAnimationsOffset = AmbientNpcAnimationsRandomOffset + 3; // Padding
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mAmbientPcAnimations;

    internal new const uint AmbientNpcAnimationsOffset = AmbientPcAnimationsOffset + 12;
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mAmbientNpcAnimations;

    internal new const uint PcIsChildOffset = AmbientNpcAnimationsOffset + 12;
    public new bool PcIsChild
    {
        get => ReadBitfield(PcIsChildOffset, 0);
        set => WriteBitfield(PcIsChildOffset, 0, value);
    }

    internal new const uint NpcIsChildOffset = PcIsChildOffset + 0;
    public new bool NpcIsChild
    {
        get => ReadBitfield(NpcIsChildOffset, 1);
        set => WriteBitfield(NpcIsChildOffset, 1, value);
    }

    internal new const uint GoToPattyAndSelmaScreenWhenDoneOffset = NpcIsChildOffset + 0;
    public new bool GoToPattyAndSelmaScreenWhenDone
    {
        get => ReadBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2);
        set => WriteBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2, value);
    }

    internal new const uint CamerasForLinesOfDialogOffset = GoToPattyAndSelmaScreenWhenDoneOffset + 4; // Padding
    // typedef std::vector< tName, s2alloc<tName> > TNAMEVECTOR;
    // TNAMEVECTOR mCamerasForLinesOfDialog;

    internal new const uint BestSideLocatorOffset = CamerasForLinesOfDialogOffset + 12;
    public new ulong BestSideLocator
    {
        get => ReadUInt64(BestSideLocatorOffset);
        set => WriteUInt64(BestSideLocatorOffset, value);
    }

    internal const uint EventListenerVFTableOffset = BestSideLocatorOffset + sizeof(ulong);

    internal const uint IconOffset = EventListenerVFTableOffset + sizeof(uint) + 8; // The fuck is the 8?
    public AnimatedIcon Icon => Memory.ClassFactory.Create<AnimatedIcon>(ReadUInt32(IconOffset));

    internal const uint AlternateIconOffset = IconOffset + sizeof(uint);
    public AnimatedIcon AlternateIcon => Memory.ClassFactory.Create<AnimatedIcon>(ReadUInt32(AlternateIconOffset));

    internal const uint MissionNumOffset = AlternateIconOffset + sizeof(uint);
    public int MissionNum
    {
        get => ReadInt32(MissionNumOffset);
        set => WriteInt32(MissionNumOffset, value);
    }

    internal const uint EventLocatorOffset = MissionNumOffset + sizeof(int);
    public EventLocator EventLocator => Memory.ClassFactory.Create<EventLocator>(ReadUInt32(EventLocatorOffset));

    internal const uint DialogEventDataOffset = EventLocatorOffset + sizeof(uint);
    public DialogEventData DialogEventData
    {
        get => ReadStruct<DialogEventData>(DialogEventDataOffset);
        set => WriteStruct(DialogEventDataOffset, value);
    }

    internal const uint IsOneShotOffset = DialogEventDataOffset + DialogEventData.Size;
    public bool IsOneShot
    {
        get => ReadBoolean(IsOneShotOffset);
        set => WriteBoolean(IsOneShotOffset, value);
    }

    internal const uint IsCompleteOffset = IsOneShotOffset + 1;
    public bool IsComplete
    {
        get => ReadBoolean(IsCompleteOffset);
        set => WriteBoolean(IsCompleteOffset, value);
    }

    internal const uint Char1PosOffset = IsCompleteOffset + 3; // Padding
    public CarStartLocator Char1Pos => Memory.ClassFactory.Create<CarStartLocator>(ReadUInt32(Char1PosOffset));

    internal const uint Char2PosOffset = Char1PosOffset + sizeof(uint);
    public CarStartLocator Char2Pos => Memory.ClassFactory.Create<CarStartLocator>(ReadUInt32(Char2PosOffset));

    internal const uint CarPosOffset = Char2PosOffset + sizeof(uint);
    public CarStartLocator CarPos => Memory.ClassFactory.Create<CarStartLocator>(ReadUInt32(CarPosOffset));

    internal const uint Char1OldPosOffset = CarPosOffset + sizeof(uint);
    public Vector3 Char1OldPos
    {
        get => ReadStruct<Vector3>(Char1OldPosOffset);
        set => WriteStruct(Char1OldPosOffset, value);
    }

    internal const uint Char2OldPosOffset = Char1OldPosOffset + Vector3.Size;
    public Vector3 Char2OldPos
    {
        get => ReadStruct<Vector3>(Char2OldPosOffset);
        set => WriteStruct(Char2OldPosOffset, value);
    }

    internal const uint CarOldPosOffset = Char2OldPosOffset + Vector3.Size;
    public Vector3 CarOldPos
    {
        get => ReadStruct<Vector3>(CarOldPosOffset);
        set => WriteStruct(CarOldPosOffset, value);
    }

    internal const uint Char1RotationOffset = CarOldPosOffset + Vector3.Size;
    public float Char1Rotation
    {
        get => ReadSingle(Char1RotationOffset);
        set => WriteSingle(Char1RotationOffset, value);
    }

    internal const uint Char2RotationOffset = Char1RotationOffset + sizeof(float);
    public float Char2Rotation
    {
        get => ReadSingle(Char2RotationOffset);
        set => WriteSingle(Char2RotationOffset, value);
    }

    internal const uint CarRotationOffset = Char2RotationOffset + sizeof(float);
    public float CarRotation
    {
        get => ReadSingle(CarRotationOffset);
        set => WriteSingle(CarRotationOffset, value);
    }

    internal const uint PreviousMissionPicOffset = CarRotationOffset + sizeof(float);
    public string PreviousMissionPic
    {
        get => ReadString(PreviousMissionPicOffset, Encoding.UTF8, 256);
        set => WriteString(PreviousMissionPicOffset, value, Encoding.UTF8, 256);
    }

    internal const uint ResetOffset = PreviousMissionPicOffset + 256;
    public bool Reset
    {
        get => ReadBitfield(ResetOffset, 0);
        set => WriteBitfield(ResetOffset, 0, value);
    }

    internal const uint MovedOffset = ResetOffset + 0;
    public bool Moved
    {
        get => ReadBitfield(MovedOffset, 1);
        set => WriteBitfield(MovedOffset, 1, value);
    }

    internal const uint HideAnimatedIconOffset = MovedOffset + 0;
    public bool HideAnimatedIcon
    {
        get => ReadBitfield(HideAnimatedIconOffset, 2);
        set => WriteBitfield(HideAnimatedIconOffset, 2, value);
    }

    internal const uint HidTheCarOffset = HideAnimatedIconOffset + 0;
    public bool HidTheCar
    {
        get => ReadBitfield(HidTheCarOffset, 3);
        set => WriteBitfield(HidTheCarOffset, 3, value);
    }

    internal const uint HidDefaultOffset = HidTheCarOffset + 0;
    public bool HidDefault
    {
        get => ReadBitfield(HidDefaultOffset, 4);
        set => WriteBitfield(HidDefaultOffset, 4, value);
    }

    internal const uint HudMapIconIDOffset = HidDefaultOffset + 4; // Padding
    public int HudMapIconID
    {
        get => ReadInt32(HudMapIconIDOffset);
        set => WriteInt32(HudMapIconIDOffset, value);
    }

    public const uint Size = HudMapIconIDOffset + sizeof(int);
}
