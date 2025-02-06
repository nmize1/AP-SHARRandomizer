using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Structs.AnimatedIcon;

[Struct(typeof(IconStruct))]
public struct Icon
{
    public const int Size = sizeof(uint) + sizeof(uint) + sizeof(int) + sizeof(uint) + sizeof(uint);

    public tDrawable Drawable;

    public Classes.tMultiController MultiController;

    public int EffectIndex;

    public tDrawable ShadowDrawable;

    public Classes.tMultiController ShadowController;

    public Icon(tDrawable drawable, Classes.tMultiController multiController, int effectIndex, tDrawable shadowDrawable, Classes.tMultiController shadowController)
    {
        Drawable = drawable;
        MultiController = multiController;
        EffectIndex = effectIndex;
        ShadowDrawable = shadowDrawable;
        ShadowController = shadowController;
    }

    public override readonly string ToString() => $"{Drawable} | {MultiController} | {EffectIndex} | {ShadowDrawable} | {ShadowController}";
}

internal class IconStruct : Struct
{
    public override int Size => Icon.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        tDrawable Drawable = Memory.ClassFactory.Create<tDrawable>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Classes.tMultiController MultiController = Memory.ClassFactory.Create<Classes.tMultiController>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        int EffectIndex = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        tDrawable ShadowDrawable = Memory.ClassFactory.Create<tDrawable>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Classes.tMultiController ShadowController = Memory.ClassFactory.Create<Classes.tMultiController>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        return new Icon(Drawable, MultiController, EffectIndex, ShadowDrawable, ShadowController);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not Icon Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(Icon)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Drawable?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.MultiController?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.EffectIndex).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.ShadowDrawable?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.ShadowController?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
    }
}
