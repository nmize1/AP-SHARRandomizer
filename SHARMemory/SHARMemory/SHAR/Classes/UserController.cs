using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVUserController@@")]
public class UserController : Class
{
    public const int MAX_PHYSICAL_BUTTONS = 53;
    public const int MAX_LOGICAL_BUTTONS = 53;
    public const int MAX_MAPPABLES = 16;
    public const int MAX_MAPPINGS = 2;
    public const int MAX_VIRTUAL_MAPPINGS = 2;

    public UserController(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IRadControllerInputPointCallbackVFTableOffset = 0;
    
    internal const uint GameConfigHandlerOffset = IRadControllerInputPointCallbackVFTableOffset + sizeof(uint);

    internal const uint ControllerIdOffset = GameConfigHandlerOffset + sizeof(uint);
    public int ControllerId
    {
        get => ReadInt32(ControllerIdOffset);
        set => WriteInt32(ControllerIdOffset, value);
    }

    internal const uint IsConnectedOffset = ControllerIdOffset + sizeof(int);
    public bool IsConnected
    {
        get => ReadBoolean(IsConnectedOffset);
        set => WriteBoolean(IsConnectedOffset, value);
    }

    internal const uint GameStateOffset = IsConnectedOffset + 4; // Padding
    public InputManager.ActiveState GameState
    {
        get => (InputManager.ActiveState)ReadUInt32(GameStateOffset);
        set => WriteUInt32(GameStateOffset, (uint)value);
    }

    internal const uint InputPointsRegisteredOffset = GameStateOffset + sizeof(uint);
    public bool InputPointsRegistered
    {
        get => ReadBoolean(InputPointsRegisteredOffset);
        set => WriteBoolean(InputPointsRegisteredOffset, value);
    }

    internal const uint ControllerOffset = InputPointsRegisteredOffset + 4;
    public PointerArray<RealController> Controller => new(Memory, Address + ControllerOffset, RealController.NUM_CONTROLLER_TYPES);

    internal const uint SteeringSpringOffset = ControllerOffset + sizeof(uint) * RealController.NUM_CONTROLLER_TYPES;
    public SHARMemory.Memory.Class SteeringSpring => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(SteeringSpringOffset));

    internal const uint SteeringDamperOffset = SteeringSpringOffset + sizeof(uint);
    public SHARMemory.Memory.Class SteeringDamper => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(SteeringDamperOffset));

    internal const uint ConstantEffectOffset = SteeringDamperOffset + sizeof(uint);
    public SHARMemory.Memory.Class ConstantEffect => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(ConstantEffectOffset));

    internal const uint WheelRumbleOffset = ConstantEffectOffset + sizeof(uint);
    public SHARMemory.Memory.Class WheelRumble => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(WheelRumbleOffset));

    internal const uint HeavyWheelRumbleOffset = WheelRumbleOffset + sizeof(uint);
    public SHARMemory.Memory.Class HeavyWheelRumble => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(HeavyWheelRumbleOffset));

    internal const uint MappableOffset = HeavyWheelRumbleOffset + sizeof(uint);
    public PointerArray<Mappable> Mappable => new(Memory, Address + MappableOffset, MAX_MAPPABLES);

    internal const uint VirtualMapOffset = MappableOffset + sizeof(uint) * MAX_MAPPABLES;
    public ClassArray<Mapper> VirtualMap => new(Memory, Address + VirtualMapOffset, Mapper.Size, MAX_VIRTUAL_MAPPINGS);

    internal const uint MapDataOffset = VirtualMapOffset + Mapper.Size * MAX_VIRTUAL_MAPPINGS;
    public ButtonMapData MapData
    {
        get => ReadStruct<ButtonMapData>(MapDataOffset);
        set => WriteStruct(MapDataOffset, value);
    }

    internal const uint NumButtonsOffset = MapDataOffset + ButtonMapData.Size;
    public int NumButtons
    {
        get => ReadInt32(NumButtonsOffset);
        set => WriteInt32(NumButtonsOffset, value);
    }

    internal const uint ButtonArrayOffset = NumButtonsOffset + sizeof(int);
    public ClassArray<Button> ButtonArray => new(Memory, Address + ButtonArrayOffset, Button.Size, MAX_PHYSICAL_BUTTONS);

    internal const uint ButtonNamesOffset = ButtonArrayOffset + Button.Size * MAX_PHYSICAL_BUTTONS;
    public StructArray<ulong> ButtonNames => new(Memory, Address + ButtonNamesOffset, sizeof(ulong), MAX_PHYSICAL_BUTTONS);

    internal const uint ButtonDeadZonesOffset = ButtonNamesOffset + sizeof(ulong) * MAX_PHYSICAL_BUTTONS;
    public StructArray<float> ButtonDeadZones => new(Memory, Address + ButtonDeadZonesOffset, sizeof(float), MAX_PHYSICAL_BUTTONS);

    internal const uint ButtonStickyOffset = ButtonDeadZonesOffset + sizeof(float) * MAX_PHYSICAL_BUTTONS;
    public StructArray<bool> ButtonSticky => new(Memory, Address + ButtonStickyOffset, sizeof(bool), MAX_PHYSICAL_BUTTONS);

    internal const uint KeyboardBackOffset = ButtonStickyOffset + MAX_PHYSICAL_BUTTONS;

    internal const uint IsRumbleOnOffset = KeyboardBackOffset + 1;

    internal const uint IsWheelOffset = IsRumbleOnOffset + 1;

    internal const uint RumbleEffectOffset = IsWheelOffset + 1;
    public RumbleEffect RumbleEffect => Memory.ClassFactory.Create<RumbleEffect>(Address + RumbleEffectOffset);

    internal const uint MouseLookOffset = RumbleEffectOffset + RumbleEffect.Size;

    internal const uint InvertMouseXOffset = MouseLookOffset + 1;

    internal const uint InvertMouseYOffset = InvertMouseXOffset + 1;

    internal const uint ForceFeedbackOffset = InvertMouseYOffset + 1;

    internal const uint TutorialDisabledOffset = ForceFeedbackOffset + 1;

    internal const uint MouseSensitivityXOffset = TutorialDisabledOffset + 4; // Padding

    internal const uint MouseSensitivityYOffset = MouseSensitivityXOffset + sizeof(float);

    internal const uint WheelSensitivityXOffset = MouseSensitivityYOffset + sizeof(float);

    internal const uint WheelSensitivityYOffset = WheelSensitivityXOffset + sizeof(float);

    internal const uint ResetMouseCounterOffset = WheelSensitivityYOffset + sizeof(float);

    public const uint Size = ResetMouseCounterOffset + sizeof(int) * 3;

    public void SetButtonValue(InputManager.Buttons button, float value, bool sticky) => SetButtonValue((int)button, value, sticky);

    public void SetButtonValue(int button, float value, bool sticky)
    {
        var buttons = ButtonArray.ToArray();
        if (button < 0 || button >= buttons.Length)
            throw new ArgumentOutOfRangeException(nameof(button), $"{nameof(button)} must be between 0 and {buttons.Length}.");

        if (ButtonSticky[button] && !sticky)
            return;

        var lastValue = buttons[button].Value;
        var deadZone = ButtonDeadZones[button];

        if (Globals.RadMathUtil.Epsilon(value, 0, deadZone))
        {
            value = 0;
        }
        else
        {
            value = Globals.RadMathUtil.Clamp(value, -1, 1);

            float sign = Math.Sign(value);
            float calibratedButtonData = Globals.RadMathUtil.Fabs(value) - deadZone;

            calibratedButtonData *= 1f / (1f - deadZone);
            calibratedButtonData *= sign;

            bool bChanged = !Globals.RadMathUtil.Epsilon(lastValue, calibratedButtonData, 0.05f);
            value = bChanged ? calibratedButtonData : lastValue;
        }

        if (value > 0 || lastValue > 0)
            ButtonArray[button].SetValue(value);

        ButtonSticky[button] = sticky;
        if (sticky && value == 0)
            ButtonSticky[button] = false;
    }

    public void DisableButton(InputManager.Buttons button)
    {
        var mapType = InputManager.ButtonToMapType(button);
        if (mapType == InputManager.MapType.Frontend)
            throw new ArgumentException($"Button {button} is a Frontend button. Disabling unsupported.");

        foreach (var virtualMap in VirtualMap)
        {
            var code = virtualMap.ButtonMap[(int)button];
            if (code == -1)
                continue;

            var buttonId = code & 0xFFFFFF;
            RealController.DirectionType dir = (RealController.DirectionType)((code >> 0x18) & 0xF);

            foreach (var controller in Controller)
                controller.DisableButton((int)mapType, buttonId, dir);
        }
    }

    public void EnableButton(InputManager.Buttons button)
    {
        var mapType = InputManager.ButtonToMapType(button);
        if (mapType == InputManager.MapType.Frontend)
            throw new ArgumentException($"Button {button} is a Frontend button. Enabling unsupported.");

        foreach (var virtualMap in VirtualMap)
        {
            var code = virtualMap.ButtonMap[(int)button];
            if (code == -1)
                continue;

            var buttonId = code & 0xFFFFFF;
            RealController.DirectionType dir = (RealController.DirectionType)((code >> 0x18) & 0xF);

            foreach (var controller in Controller)
                controller.EnableButton((int)mapType, buttonId, dir, button);
        }
    }
}
