using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVMouse@@")]
public class Mouse : RealController
{
    public const uint NUM_MOUSE_BUTTONS = 11;

    private const int DIMOFS_X = 0;
    private const int DIMOFS_Y = 4;
    private const int DIMOFS_Z = 8;
    private const int DIMOFS_BUTTON0 = 12;
    private const int DIMOFS_BUTTON1 = 13;
    private const int DIMOFS_BUTTON2 = 14;
    private const int DIMOFS_BUTTON3 = 15;
    private const int DIMOFS_BUTTON4 = 12;
    private const int DIMOFS_BUTTON5 = 13;
    private const int DIMOFS_BUTTON6 = 14;
    private const int DIMOFS_BUTTON7 = 15;
    private static int KeyToMouseMap(int dxKey)
    {
        return dxKey switch
        {
            _ when dxKey == DIMOFS_X => 0,
            _ when dxKey == DIMOFS_Y => 1,
            _ when dxKey == DIMOFS_Z => 2,
            _ when dxKey == DIMOFS_BUTTON0 => 3,
            _ when dxKey == DIMOFS_BUTTON1 => 4,
            _ when dxKey == DIMOFS_BUTTON2 => 5,
            _ when dxKey == DIMOFS_BUTTON3 => 6,
            _ when dxKey == DIMOFS_BUTTON4 => 7,
            _ when dxKey == DIMOFS_BUTTON5 => 8,
            _ when dxKey == DIMOFS_BUTTON6 => 9,
            _ when dxKey == DIMOFS_BUTTON7 => 10,
            _ => (int)NUM_MOUSE_BUTTONS,
        };
    }

    public Mouse(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint RelativePositionOffset = NumInputPointsOffset + sizeof(int);
    public MouseCoord RelativePosition
    {
        get => ReadStruct<MouseCoord>(RelativePositionOffset);
        set => WriteStruct(RelativePositionOffset, value);
    }

    internal const uint AbsolutePositionOffset = RelativePositionOffset + MouseCoord.Size;
    public MouseCoord AbsolutePosition
    {
        get => ReadStruct<MouseCoord>(AbsolutePositionOffset);
        set => WriteStruct(AbsolutePositionOffset, value);
    }

    internal const uint SensitivityXOffset = AbsolutePositionOffset + MouseCoord.Size;
    public float SensitivityX
    {
        get => ReadSingle(SensitivityXOffset);
        set => WriteSingle(SensitivityXOffset, value);
    }

    internal const uint SensitivityYOffset = SensitivityXOffset + sizeof(float);
    public float SensitivityY
    {
        get => ReadSingle(SensitivityYOffset);
        set => WriteSingle(SensitivityYOffset, value);
    }

    internal const uint ButtonMapOffset = SensitivityYOffset + sizeof(float);
    internal const int ButtonMapSize = (int)(InputManager.NUM_MAPTYPES * NUM_MOUSE_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int));
    public int[,,] ButtonMap
    {
        get
        {
            var buttonMap = new int[InputManager.NUM_MAPTYPES, NUM_MOUSE_BUTTONS, NUM_DIRECTION_TYPES];
            var bytes = ReadBytes(ButtonMapOffset, ButtonMapSize);
            Buffer.BlockCopy(bytes, 0, buttonMap, 0, ButtonMapSize);
            return buttonMap;
        }
        set
        {
            if (value.GetLength(0) != InputManager.NUM_MAPTYPES || value.GetLength(1) != NUM_MOUSE_BUTTONS || value.GetLength(2) != NUM_DIRECTION_TYPES)
                throw new ArgumentException($"Invalid array dimensions! Expected [{InputManager.NUM_MAPTYPES},{NUM_MOUSE_BUTTONS},{NUM_DIRECTION_TYPES}].");

            var bytes = new byte[ButtonMapSize];
            Buffer.BlockCopy(value, 0, bytes, 0, ButtonMapSize);
            WriteBytes(ButtonMapOffset, bytes);
        }
    }

    public override void DisableButton(int mapType, int buttonId)
    {
        if (mapType < 0 || mapType > InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        var mouseButton = KeyToMouseMap(buttonId);
        if (mouseButton == NUM_MOUSE_BUTTONS)
            return;

        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_MOUSE_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int) + mouseButton * NUM_DIRECTION_TYPES * sizeof(int)), -1);
    }

    public override void EnableButton(int mapType, int buttonId, InputManager.Buttons button)
    {
        if (mapType < 0 || mapType > InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        var mouseButton = KeyToMouseMap(buttonId);
        if (mouseButton == NUM_MOUSE_BUTTONS)
            return;

        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_MOUSE_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int) + mouseButton * NUM_DIRECTION_TYPES * sizeof(int)), (int)button);
    }
}
