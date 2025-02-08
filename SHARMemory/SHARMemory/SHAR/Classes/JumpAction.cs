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

    internal const uint BitfieldOffset = RootDriverOffset + sizeof(uint);
    private byte Bitfield
    {
        get => ReadByte(BitfieldOffset);
        set => WriteByte(BitfieldOffset, value);
    }

    public bool Boost
    {
        get => (Bitfield & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00000001;
            else
                Bitfield &= 0b11111110;
        }
    }

    public bool Falling
    {
        get => (Bitfield & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00000010;
            else
                Bitfield &= 0b11111101;
        }
    }

    public bool JumpAgain
    {
        get => (Bitfield & 0b00000100) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00000100;
            else
                Bitfield &= 0b11111011;
        }
    }

    public bool TurboJump
    {
        get => (Bitfield & 0b00001000) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00001000;
            else
                Bitfield &= 0b11110111;
        }
    }

    public bool InJumpKick
    {
        get => (Bitfield & 0b00010000) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00010000;
            else
                Bitfield &= 0b11101111;
        }
    }

    public bool DoJumpKick
    {
        get => (Bitfield & 0b00100000) != 0;
        set
        {
            if (value)
                Bitfield |= 0b00100000;
            else
                Bitfield &= 0b11011111;
        }
    }

    public bool Slam
    {
        get => (Bitfield & 0b01000000) != 0;
        set
        {
            if (value)
                Bitfield |= 0b01000000;
            else
                Bitfield &= 0b10111111;
        }
    }

    public bool PreSlam
    {
        get => (Bitfield & 0b10000000) != 0;
        set
        {
            if (value)
                Bitfield |= 0b10000000;
            else
                Bitfield &= 0b01111111;
        }
    }
}