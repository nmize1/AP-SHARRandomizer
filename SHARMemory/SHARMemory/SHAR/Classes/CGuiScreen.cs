using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiScreen@@")]
public class CGuiScreen : CGuiWindow
{
    public const int MAX_FOREGROUND_LAYERS = 8;
    public const int MAX_BACKGROUND_LAYERS = 2;
    public const int NUM_BUTTON_ICONS = 2;

    public enum IrisState
    {
        Idle,
        Closing,
        Closed,
        Opening,
    }

    public CGuiScreen(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint GuiManagerOffset = FirstTimeEnteredOffset + 4; // Padding
    public CGuiManager GuiManager => Memory.ClassFactory.Create<CGuiManager>(ReadUInt32(GuiManagerOffset));

    internal const uint ScroobyScreenOffset = GuiManagerOffset + sizeof(uint);
    public FeEntity ScroobyScreen => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(ScroobyScreenOffset));

    internal const uint ScreenCoverOffset = ScroobyScreenOffset + sizeof(uint);
    public FeEntity ScreenCover => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(ScreenCoverOffset));

    internal const uint P3DObjectOffset = ScreenCoverOffset + sizeof(uint);
    public FeEntity P3DObject => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(P3DObjectOffset));

    internal const uint P3DIrisOffset = P3DObjectOffset + sizeof(uint);
    public FeEntity P3DIris => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(P3DIrisOffset));

    internal const uint IrisControllerOffset = P3DIrisOffset + sizeof(uint);
    public tMultiController IrisController => Memory.ClassFactory.Create<tMultiController>(ReadUInt32(IrisControllerOffset));

    internal const uint CurrentIrisStateOffset = IrisControllerOffset + sizeof(uint);
    public IrisState CurrentIrisState
    {
        get => (IrisState)ReadInt32(CurrentIrisStateOffset);
        set => WriteInt32(CurrentIrisStateOffset, (int)value);
    }

    internal const uint AutoOpenIrisOffset = CurrentIrisStateOffset + sizeof(int);
    public bool AutoOpenIris
    {
        get => ReadBoolean(AutoOpenIrisOffset);
        set => WriteBoolean(AutoOpenIrisOffset, value);
    }

    internal const uint ForegroundLayersOffset = AutoOpenIrisOffset + 4; // Padding
    public PointerArray<FeEntity> ForegroundLayers => new(Memory, Address + ForegroundLayersOffset, MAX_FOREGROUND_LAYERS);

    internal const uint NumForegroundLayersOffset = ForegroundLayersOffset + sizeof(uint) * MAX_FOREGROUND_LAYERS;
    public int NumForegroundLayers
    {
        get => ReadInt32(NumForegroundLayersOffset);
        set => WriteInt32(NumForegroundLayersOffset, value);
    }

    internal const uint BackgroundLayersOffset = NumForegroundLayersOffset + sizeof(int);
    public PointerArray<FeEntity> BackgroundLayers => new(Memory, Address + BackgroundLayersOffset, MAX_BACKGROUND_LAYERS);

    internal const uint NumBackgroundLayersOffset = BackgroundLayersOffset + sizeof(uint) * MAX_BACKGROUND_LAYERS;
    public int NumBackgroundLayers
    {
        get => ReadInt32(NumBackgroundLayersOffset);
        set => WriteInt32(NumBackgroundLayersOffset, value);
    }

    internal const uint ButtonIconsOffset = NumBackgroundLayersOffset + sizeof(int);
    public PointerArray<FeEntity> ButtonIcons => new(Memory, Address + ButtonIconsOffset, NUM_BUTTON_ICONS);

    internal const uint Bitfield_0x74OFfset = ButtonIconsOffset + sizeof(uint) * NUM_BUTTON_ICONS;
    private byte Bitfield_0x74
    {
        get => ReadByte(Bitfield_0x74OFfset);
        set => WriteByte(Bitfield_0x74OFfset, value);
    }

    public bool IgnoreControllerInputs
    {
        get => (Bitfield_0x74 & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield_0x74 |= 0b00000001;
            else
                Bitfield_0x74 &= 0b11111110;
        }
    }

    public bool InverseFading
    {
        get => (Bitfield_0x74 & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield_0x74 |= 0b00000010;
            else
                Bitfield_0x74 &= 0b11111101;
        }
    }

    internal const uint ScreenFXOffset = Bitfield_0x74OFfset + 4; // Padding
    public uint ScreenFX
    {
        get => ReadUInt32(ScreenFXOffset);
        set => WriteUInt32(ScreenFXOffset, value);
    }

    internal const uint FadeTimeOffset = ScreenFXOffset + sizeof(uint);
    public float FadeTime
    {
        get => ReadSingle(FadeTimeOffset);
        set => WriteSingle(FadeTimeOffset, value);
    }

    internal const uint ElapsedFadeTimeOffset = FadeTimeOffset + sizeof(float);
    public float ElapsedFadeTime
    {
        get => ReadSingle(ElapsedFadeTimeOffset);
        set => WriteSingle(ElapsedFadeTimeOffset, value);
    }

    internal const uint ZoomTimeOffset = ElapsedFadeTimeOffset + sizeof(float);
    public float ZoomTime
    {
        get => ReadSingle(ZoomTimeOffset);
        set => WriteSingle(ZoomTimeOffset, value);
    }

    internal const uint ElapsedZoomTimeOffset = ZoomTimeOffset + sizeof(float);
    public float ElapsedZoomTime
    {
        get => ReadSingle(ElapsedZoomTimeOffset);
        set => WriteSingle(ElapsedZoomTimeOffset, value);
    }

    internal const uint SlideTimeOffset = ElapsedZoomTimeOffset + sizeof(float);
    public float SlideTime
    {
        get => ReadSingle(SlideTimeOffset);
        set => WriteSingle(SlideTimeOffset, value);
    }

    internal const uint ElapsedSlideTimeOffset = SlideTimeOffset + sizeof(float);
    public float ElapsedSlideTime
    {
        get => ReadSingle(ElapsedSlideTimeOffset);
        set => WriteSingle(ElapsedSlideTimeOffset, value);
    }

    internal const uint PlayTransitionAnimationLastOffset = ElapsedSlideTimeOffset + sizeof(float);
    public bool PlayTransitionAnimationLast
    {
        get => ReadBoolean(PlayTransitionAnimationLastOffset);
        set => WriteBoolean(PlayTransitionAnimationLastOffset, value);
    }
}
