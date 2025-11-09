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

    internal const uint ScreenCoverOffset = ScroobyScreenOffset + sizeof(uint);

    internal const uint P3DObject = ScreenCoverOffset + sizeof(uint);

    internal const uint P3DIris = P3DObject + sizeof(uint);

    internal const uint IrisControllerOffset = P3DIris + sizeof(uint);
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

    internal const uint NumForegroundLayersOffset = ForegroundLayersOffset + sizeof(uint) * MAX_FOREGROUND_LAYERS;
    public int NumForegroundLayers
    {
        get => ReadInt32(NumForegroundLayersOffset);
        set => WriteInt32(NumForegroundLayersOffset, value);
    }

    internal const uint BackgroundLayersOffset = NumForegroundLayersOffset + sizeof(int);

    internal const uint NumBackgroundLayersOffset = BackgroundLayersOffset + sizeof(uint) * MAX_BACKGROUND_LAYERS;
    public int NumBackgroundLayers
    {
        get => ReadInt32(NumBackgroundLayersOffset);
        set => WriteInt32(NumBackgroundLayersOffset, value);
    }

    internal const uint ButtonIconsOffset = NumBackgroundLayersOffset + sizeof(int);

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
