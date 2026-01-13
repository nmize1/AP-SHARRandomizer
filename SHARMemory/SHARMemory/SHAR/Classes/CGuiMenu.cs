using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;
using System.Drawing;
using static SHARMemory.SHAR.Classes.CGuiManager;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiMenu@@")]
public class CGuiMenu : CGuiEntity
{
    public static readonly Color DEFAULT_DISABLED_ITEM_COLOUR = Color.FromArgb(128, 128, 128);

    public enum MenuTypes
    {
        Text,
        Sprite,
    }

    [Flags]
    public enum SpecialEffect : short
    {
        None,
        ColourPulse,
        SizePulse,
        All = ~0
    }

    public CGuiMenu(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint MenuTypeOffset = ParentOffset + sizeof(uint);
    public MenuTypes MenuType
    {
        get => (MenuTypes)ReadInt32(MenuTypeOffset);
        set => WriteInt32(MenuTypeOffset, (int)value);
    }

    internal const uint SpecialEffectsOffset = MenuTypeOffset + sizeof(int);
    public SpecialEffect SpecialEffects
    {
        get => (SpecialEffect)ReadInt16(SpecialEffectsOffset);
        set => WriteInt16(SpecialEffectsOffset, (short)value);
    }

    internal const uint MenuItemsOffset = SpecialEffectsOffset + sizeof(short) + 2; // Padding
    public PointerArray<GuiMenuItem> MenuItems => new(Memory, ReadUInt32(MenuItemsOffset), NumItems);

    internal const uint NumItemsOffset = MenuItemsOffset + sizeof(uint);
    public int NumItems
    {
        get => ReadInt32(NumItemsOffset);
        set => WriteInt32(NumItemsOffset, value);
    }
}
