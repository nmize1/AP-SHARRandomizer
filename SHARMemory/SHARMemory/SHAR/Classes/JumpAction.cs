using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVJumpAction@@")]
public class JumpAction : SHARTask
{
    public enum JumpStates
    {
        NoTranslate = 1 << 16,
        InitJump,
        PostTurboJumpRecover,
        IdleJump,

        AllowTranslate = 1 << 17,
        PreJump,
        Jump,
        PostJump,
        JumpDone
    }

    public JumpAction(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CharacterOffset = StatusOffset + sizeof(uint);
    public Character Character => Memory.ClassFactory.Create<Character>(ReadUInt32(CharacterOffset));

    internal const uint JumpStateOffset = CharacterOffset + sizeof(uint);
    public JumpStates JumpState
    {
        get => (JumpStates)ReadUInt32(JumpStateOffset);
        set => WriteUInt32(JumpStateOffset, (uint)value);
    }

    internal const uint VelocityOffset = JumpStateOffset + sizeof(uint);
    public Vector3 Velocity
    {
        get => ReadStruct<Vector3>(VelocityOffset);
        set => WriteStruct(VelocityOffset, value);
    }

    internal const uint AnimUidOffset = VelocityOffset + Vector3.Size;
    public ulong AnimUid
    {
        get => ReadUInt64(AnimUidOffset);
        set => WriteUInt64(AnimUidOffset, value);
    }

    internal const uint TimeOffset = AnimUidOffset + sizeof(ulong);
    public float Time
    {
        get => ReadSingle(TimeOffset);
        set => WriteSingle(TimeOffset, value);
    }

    internal const uint GravityOffset = TimeOffset + sizeof(float);
    public float Gravity
    {
        get => ReadSingle(GravityOffset);
        set => WriteSingle(GravityOffset, value);
    }

    internal const uint StartHeightOffset = GravityOffset + sizeof(float);
    public float StartHeight
    {
        get => ReadSingle(StartHeightOffset);
        set => WriteSingle(StartHeightOffset, value);
    }

    internal const uint JumpHeightOffset = StartHeightOffset + sizeof(float);
    public float JumpHeight
    {
        get => ReadSingle(JumpHeightOffset);
        set => WriteSingle(JumpHeightOffset, value);
    }

    internal const uint MaxSpeedOffset = JumpHeightOffset + sizeof(float);
    public float MaxSpeed
    {
        get => ReadSingle(MaxSpeedOffset);
        set => WriteSingle(MaxSpeedOffset, value);
    }

    internal const uint AnimationDriverOffset = MaxSpeedOffset + sizeof(float);

    internal const uint RootControllerOffset = AnimationDriverOffset + sizeof(uint);

    internal const uint RootDriverOffset = RootControllerOffset + sizeof(uint);

    internal const uint BoostOffset = RootDriverOffset + sizeof(uint);
    public bool Boost
    {
        get => ReadBitfield(BoostOffset, 0);
        set => WriteBitfield(BoostOffset, 0, value);
    }

    internal const uint FallingOffset = BoostOffset + 0;
    public bool Falling
    {
        get => ReadBitfield(FallingOffset, 1);
        set => WriteBitfield(FallingOffset, 1, value);
    }

    internal const uint JumpAgainOffset = FallingOffset + 0;
    public bool JumpAgain
    {
        get => ReadBitfield(JumpAgainOffset, 2);
        set => WriteBitfield(JumpAgainOffset, 2, value);
    }

    internal const uint TurboJumpOffset = JumpAgainOffset + 0;
    public bool TurboJump
    {
        get => ReadBitfield(TurboJumpOffset, 3);
        set => WriteBitfield(TurboJumpOffset, 3, value);
    }

    internal const uint InJumpKickOffset = TurboJumpOffset + 0;
    public bool InJumpKick
    {
        get => ReadBitfield(InJumpKickOffset, 4);
        set => WriteBitfield(InJumpKickOffset, 4, value);
    }

    internal const uint DoJumpKickOffset = InJumpKickOffset + 0;
    public bool DoJumpKick
    {
        get => ReadBitfield(DoJumpKickOffset, 5);
        set => WriteBitfield(DoJumpKickOffset, 5, value);
    }

    internal const uint SlamOffset = DoJumpKickOffset + 0;
    public bool Slam
    {
        get => ReadBitfield(SlamOffset, 6);
        set => WriteBitfield(SlamOffset, 6, value);
    }

    internal const uint PreSlamOffset = SlamOffset + 0;
    public bool PreSlam
    {
        get => ReadBitfield(PreSlamOffset, 7);
        set => WriteBitfield(PreSlamOffset, 7, value);
    }
}