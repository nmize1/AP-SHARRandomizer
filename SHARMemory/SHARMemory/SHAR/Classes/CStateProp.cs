using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCStateProp@@")]
public class CStateProp : tEntity
{
    public const int MaxListeners = 10;

    public CStateProp(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    private const uint StatePropDataOffset = NameOffset + sizeof(long);
    public SHARMemory.Memory.Class StatePropData => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(StatePropDataOffset));

    private const uint AnimatedObjectOffset = StatePropDataOffset + sizeof(uint);
    public SHARMemory.Memory.Class AnimatedObject => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(AnimatedObjectOffset));

    private const uint BaseFrameControllerOffset = AnimatedObjectOffset + sizeof(uint);
    public SHARMemory.Memory.Class BaseFrameController => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(BaseFrameControllerOffset));

    private const uint FastDisplayDrawableOffset = BaseFrameControllerOffset + sizeof(uint);
    public tDrawable FastDisplayDrawable => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(FastDisplayDrawableOffset));

    private const uint CurrentStateOffset = FastDisplayDrawableOffset + sizeof(uint);
    public uint CurrentState
    {
        get => ReadUInt32(CurrentStateOffset);
        set => WriteUInt32(CurrentStateOffset, value);
    }

    private const uint NumStatePropListenersOffset = CurrentStateOffset + sizeof(uint);
    public uint NumStatePropListeners
    {
        get => ReadUInt32(NumStatePropListenersOffset);
        set => WriteUInt32(NumStatePropListenersOffset, value);
    }

    private const uint StatePropListenersOffset = NumStatePropListenersOffset + sizeof(uint);
    public PointerArray<SHARMemory.Memory.Class> StatePropListener => new(Memory, Address + StatePropListenersOffset, MaxListeners);
}
