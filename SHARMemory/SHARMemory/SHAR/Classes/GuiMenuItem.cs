using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;
using System.Drawing;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AUGuiMenuItem@@")]
public class GuiMenuItem : Class
{
    [Flags]
    public enum Attribute
    {
        None = 0,
        SelectionEnabled = 1,
        Selectable = 2,
        ValuesWrapped = 4,
        TextOutlineEnabled = 8,
        All = -1
    }

    public GuiMenuItem(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint GuiMenuItemVFTableOffset = 0;

    internal const uint AttributesOffset = GuiMenuItemVFTableOffset + sizeof(uint);
    public Attribute Attributes
    {
        get => (Attribute)ReadInt32(AttributesOffset);
        set => WriteInt32(AttributesOffset, (int)value);
    }

    internal const uint DefaultColourOffset = AttributesOffset + sizeof(int);
    public Color DefaultColour
    {
        get => ReadStruct<Color>(DefaultColourOffset);
        set => WriteStruct(DefaultColourOffset, value);
    }

    internal const uint ItemValueArrowLOffset = DefaultColourOffset + sizeof(uint);
    public FeEntity ItemValueArrowL => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(ItemValueArrowLOffset));

    internal const uint ItemValueArrowROffset = ItemValueArrowLOffset + sizeof(uint);
    public FeEntity ItemValueArrowR => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(ItemValueArrowROffset));

    internal const uint ItemValueCountOffset = ItemValueArrowROffset + sizeof(uint);
    public int ItemValueCount
    {
        get => ReadInt32(ItemValueCountOffset);
        set => WriteInt32(ItemValueCountOffset, value);
    }

    internal const uint SliderOffset = ItemValueCountOffset + sizeof(int);
    //public ImageSlider Slider
    //{
    //    get => ReadStruct<ImageSlider>(SliderOffset);
    //    set => WriteStruct(SliderOffset, value);
    //}
}
