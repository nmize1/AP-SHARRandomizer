using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVKeyboard@@")]
public class Keyboard : RealController
{
    public const uint NUM_KEYBOARD_BUTTONS = 256;

    public Keyboard(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ButtonMapOffset = NumInputPointsOffset + sizeof(int);
    internal const int ButtonMapSize = (int)(InputManager.NUM_MAPTYPES * NUM_KEYBOARD_BUTTONS * sizeof(int));
    public int[,] ButtonMap
    {
        get
        {
            var buttonMap = new int[InputManager.NUM_MAPTYPES, NUM_KEYBOARD_BUTTONS];
            var bytes = ReadBytes(ButtonMapOffset, ButtonMapSize);
            Buffer.BlockCopy(bytes, 0, buttonMap, 0, ButtonMapSize);
            return buttonMap;
        }
        set
        {
            if (value.GetLength(0) != InputManager.NUM_MAPTYPES || value.GetLength(1) != NUM_KEYBOARD_BUTTONS)
                throw new ArgumentException($"Invalid array dimensions! Expected [{InputManager.NUM_MAPTYPES},{NUM_KEYBOARD_BUTTONS}].");

            var bytes = new byte[ButtonMapSize];
            Buffer.BlockCopy(value, 0, bytes, 0, ButtonMapSize);
            WriteBytes(ButtonMapOffset, bytes);
        }
    }

    public override void DisableButton(int mapType, int buttonId)
    {
        if (mapType < 0 || mapType > InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        if (buttonId < 0 || buttonId > NUM_KEYBOARD_BUTTONS)
            throw new ArgumentOutOfRangeException(nameof(buttonId), $"{nameof(buttonId)} must be greater than 0 and less than {NUM_KEYBOARD_BUTTONS}.");

        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_KEYBOARD_BUTTONS * sizeof(int) + buttonId * sizeof(int)), -1);
    }

    public override void EnableButton(int mapType, int buttonId, InputManager.Buttons button)
    {
        if (mapType < 0 || mapType > InputManager.NUM_MAPTYPES)
            throw new ArgumentOutOfRangeException(nameof(mapType), $"{nameof(mapType)} must be greater than 0 and less than {InputManager.NUM_MAPTYPES}.");

        if (buttonId < 0 || buttonId > NUM_KEYBOARD_BUTTONS)
            throw new ArgumentOutOfRangeException(nameof(buttonId), $"{nameof(buttonId)} must be greater than 0 and less than {NUM_KEYBOARD_BUTTONS}.");
        
        WriteInt32((uint)(ButtonMapOffset + mapType * NUM_KEYBOARD_BUTTONS * sizeof(int) + buttonId * sizeof(int)), (int)button);
    }
}
