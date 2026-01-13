using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiScreenMissionSelect@@")]
public class CGuiScreenMissionSelect : CGuiScreen
{
    public const int MAX_NUM_REGULAR_MISSIONS = 7;

    public CGuiScreenMissionSelect(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint MenuLevelOffset = PlayTransitionAnimationLastOffset + 4; // Padding
    public CGuiMenu MenuLevel => Memory.ClassFactory.Create<CGuiMenu>(ReadUInt32(MenuLevelOffset));

    internal const uint MenuOffset = MenuLevelOffset + sizeof(uint);
    public CGuiMenu Menu => Memory.ClassFactory.Create<CGuiMenu>(ReadUInt32(MenuOffset));

    internal const uint NumLevelSelectionsOffset = MenuOffset + sizeof(uint);
    public int NumLevelSelections
    {
        get => ReadInt32(NumLevelSelectionsOffset);
        set => WriteInt32(NumLevelSelectionsOffset, value);
    }

    internal const uint LeftArrowOffset = NumLevelSelectionsOffset + sizeof(int);
    public FeEntity LeftArrow => Memory.ClassFactory.Create<FeEntity>(LeftArrowOffset);

    internal const uint RightArrowOffset = LeftArrowOffset + sizeof(uint);
    public FeEntity RightArrow => Memory.ClassFactory.Create<FeEntity>(RightArrowOffset);

    internal const uint MissionInfoOffset = RightArrowOffset + sizeof(uint);
    public StructArray<MissionDisplayInfo> MissionInfo => new(Memory, Address + MissionInfoOffset, MissionDisplayInfo.Size, MAX_NUM_REGULAR_MISSIONS);
}
