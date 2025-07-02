using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVGamepad@@")]
public class Gamepad : RealController
{
    public const uint NUM_GAMEPAD_BUTTONS = 42;

    private const int DIJOFS_X = 0;
    private const int DIJOFS_Y = 4;
    private const int DIJOFS_Z = 8;
    private const int DIJOFS_RX = 12;
    private const int DIJOFS_RY = 16;
    private const int DIJOFS_RZ = 20;
    private const int DIJOFS_SLIDER0 = 24;
    private const int DIJOFS_SLIDER1 = 28;
    private const int DIJOFS_POV0 = 32;
    private const int DIJOFS_POV1 = 36;
    private const int DIJOFS_BUTTON0 = 48;
    private const int DIJOFS_BUTTON1 = 49;
    private const int DIJOFS_BUTTON2 = 50;
    private const int DIJOFS_BUTTON3 = 51;
    private const int DIJOFS_BUTTON4 = 52;
    private const int DIJOFS_BUTTON5 = 53;
    private const int DIJOFS_BUTTON6 = 54;
    private const int DIJOFS_BUTTON7 = 55;
    private const int DIJOFS_BUTTON8 = 56;
    private const int DIJOFS_BUTTON9 = 57;
    private const int DIJOFS_BUTTON10 = 58;
    private const int DIJOFS_BUTTON11 = 59;
    private const int DIJOFS_BUTTON12 = 60;
    private const int DIJOFS_BUTTON13 = 61;
    private const int DIJOFS_BUTTON14 = 62;
    private const int DIJOFS_BUTTON15 = 63;
    private const int DIJOFS_BUTTON16 = 64;
    private const int DIJOFS_BUTTON17 = 65;
    private const int DIJOFS_BUTTON18 = 66;
    private const int DIJOFS_BUTTON19 = 67;
    private const int DIJOFS_BUTTON20 = 68;
    private const int DIJOFS_BUTTON21 = 69;
    private const int DIJOFS_BUTTON22 = 70;
    private const int DIJOFS_BUTTON23 = 71;
    private const int DIJOFS_BUTTON24 = 72;
    private const int DIJOFS_BUTTON25 = 73;
    private const int DIJOFS_BUTTON26 = 74;
    private const int DIJOFS_BUTTON27 = 75;
    private const int DIJOFS_BUTTON28 = 76;
    private const int DIJOFS_BUTTON29 = 77;
    private const int DIJOFS_BUTTON30 = 78;
    private const int DIJOFS_BUTTON31 = 79;
    private static int KeyToGamepadMap(int dxKey)
    {
        return dxKey switch
        {
            _ when dxKey == DIJOFS_X => 0,
            _ when dxKey == DIJOFS_Y => 1,
            _ when dxKey == DIJOFS_Z => 2,
            _ when dxKey == DIJOFS_RX => 3,
            _ when dxKey == DIJOFS_RY => 4,
            _ when dxKey == DIJOFS_RZ => 5,
            _ when dxKey == DIJOFS_SLIDER0 => 6,
            _ when dxKey == DIJOFS_SLIDER1 => 7,
            _ when dxKey == DIJOFS_POV0 => 8,
            _ when dxKey == DIJOFS_POV1 => 9,
            _ when dxKey == DIJOFS_BUTTON0 => 10,
            _ when dxKey == DIJOFS_BUTTON1 => 11,
            _ when dxKey == DIJOFS_BUTTON2 => 12,
            _ when dxKey == DIJOFS_BUTTON3 => 13,
            _ when dxKey == DIJOFS_BUTTON4 => 14,
            _ when dxKey == DIJOFS_BUTTON5 => 15,
            _ when dxKey == DIJOFS_BUTTON6 => 16,
            _ when dxKey == DIJOFS_BUTTON7 => 17,
            _ when dxKey == DIJOFS_BUTTON8 => 18,
            _ when dxKey == DIJOFS_BUTTON9 => 19,
            _ when dxKey == DIJOFS_BUTTON10 => 20,
            _ when dxKey == DIJOFS_BUTTON11 => 21,
            _ when dxKey == DIJOFS_BUTTON12 => 22,
            _ when dxKey == DIJOFS_BUTTON13 => 23,
            _ when dxKey == DIJOFS_BUTTON14 => 24,
            _ when dxKey == DIJOFS_BUTTON15 => 25,
            _ when dxKey == DIJOFS_BUTTON16 => 26,
            _ when dxKey == DIJOFS_BUTTON17 => 27,
            _ when dxKey == DIJOFS_BUTTON18 => 28,
            _ when dxKey == DIJOFS_BUTTON19 => 29,
            _ when dxKey == DIJOFS_BUTTON20 => 30,
            _ when dxKey == DIJOFS_BUTTON21 => 31,
            _ when dxKey == DIJOFS_BUTTON22 => 32,
            _ when dxKey == DIJOFS_BUTTON23 => 33,
            _ when dxKey == DIJOFS_BUTTON24 => 34,
            _ when dxKey == DIJOFS_BUTTON25 => 35,
            _ when dxKey == DIJOFS_BUTTON26 => 36,
            _ when dxKey == DIJOFS_BUTTON27 => 37,
            _ when dxKey == DIJOFS_BUTTON28 => 38,
            _ when dxKey == DIJOFS_BUTTON29 => 39,
            _ when dxKey == DIJOFS_BUTTON30 => 40,
            _ when dxKey == DIJOFS_BUTTON31 => 41,
            _ => (int)NUM_GAMEPAD_BUTTONS,
        };
    }

    public Gamepad(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ButtonMapOffset = NumInputPointsOffset + sizeof(float);
    internal const int ButtonMapSize = (int)(InputManager.NUM_MAPTYPES * NUM_GAMEPAD_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int));
    public int[,,] ButtonMap
    {
        get
        {
            var buttonMap = new int[InputManager.NUM_MAPTYPES, NUM_GAMEPAD_BUTTONS, NUM_DIRECTION_TYPES];
            var bytes = ReadBytes(ButtonMapOffset, ButtonMapSize);
            Buffer.BlockCopy(bytes, 0, buttonMap, 0, ButtonMapSize);
            return buttonMap;
        }
        set
        {
            if (value.GetLength(0) != InputManager.NUM_MAPTYPES || value.GetLength(1) != NUM_GAMEPAD_BUTTONS || value.GetLength(2) != NUM_DIRECTION_TYPES)
                throw new ArgumentException($"Invalid array dimensions! Expected [{InputManager.NUM_MAPTYPES},{NUM_GAMEPAD_BUTTONS},{NUM_DIRECTION_TYPES}].");

            var bytes = new byte[ButtonMapSize];
            Buffer.BlockCopy(value, 0, bytes, 0, ButtonMapSize);
            WriteBytes(ButtonMapOffset, bytes);
        }
    }

    public override void DisableButton(int mapType, int buttonId, DirectionType dir)
    {
        if (mapType < 0 || mapType >= InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        var gamepadButton = KeyToGamepadMap(buttonId);
        if (gamepadButton == NUM_GAMEPAD_BUTTONS)
            return;

        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_GAMEPAD_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int) + gamepadButton * NUM_DIRECTION_TYPES * sizeof(int) + (int)dir * sizeof(int)), -1);
    }

    public override void EnableButton(int mapType, int buttonId, DirectionType dir, InputManager.Buttons button)
    {
        if (mapType < 0 || mapType >= InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        var gamepadButton = KeyToGamepadMap(buttonId);
        if (gamepadButton == NUM_GAMEPAD_BUTTONS)
            return;

        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_GAMEPAD_BUTTONS * NUM_DIRECTION_TYPES * sizeof(int) + gamepadButton * NUM_DIRECTION_TYPES * sizeof(int) + (int)dir * sizeof(int)), (int)button);
    }
}
