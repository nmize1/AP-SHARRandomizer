using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using static SHARMemory.SHAR.Classes.CGuiManager;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiWindow@@")]
public class CGuiWindow : CGuiEntity
{
    public enum WindowState
    {
        Uninitialized,
        Intro,
        Running,
        Paused,
        Idle,
        Outro,
        Disabled,
    }

    public CGuiWindow(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint StateOffset = ParentOffset + sizeof(uint);
    public WindowState State
    {
        get => (WindowState)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint PrevStateOffset = StateOffset + sizeof(uint);
    public WindowState PrevState
    {
        get => (WindowState)ReadUInt32(PrevStateOffset);
        set => WriteUInt32(PrevStateOffset, (uint)value);
    }

    internal const uint IDOffset = PrevStateOffset + sizeof(uint);
    public WindowID ID
    {
        get => (WindowID)ReadInt32(IDOffset);
        set => WriteInt32(IDOffset, (int)value);
    }

    internal const uint NumTranspitionsPendingOffset = IDOffset + sizeof(int);
    public int NumTranspitionsPending
    {
        get => ReadInt32(NumTranspitionsPendingOffset);
        set => WriteInt32(NumTranspitionsPendingOffset, value);
    }

    internal const uint FirstTimeEnteredOffset = NumTranspitionsPendingOffset + sizeof(int);
    public bool FirstTimeEntered
    {
        get => ReadBoolean(FirstTimeEnteredOffset);
        set => WriteBoolean(FirstTimeEnteredOffset, value);
    }
}
