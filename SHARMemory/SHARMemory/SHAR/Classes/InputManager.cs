﻿using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;
using System.Collections.Generic;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVInputManager@@")]
public class InputManager : Class
{
    public const int MAX_CONTROLLERS = 4;
    public const int MAX_PLAYERS = 4;
    public const int NUM_MAPTYPES = 3;

    public enum Buttons : uint
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Attack,
        Jump,
        Sprint,
        DoAction,
        Accelerate,
        Reverse,
        SteerLeft,
        SteerRight,
        GetOutCar,
        HandBrake,
        Horn,
        ResetCar,
        CameraLeft,
        CameraRight,
        CameraMoveIn,
        CameraMoveOut,
        CameraZoom,
        CameraLookUp,
        CameraCarLeft,
        CameraCarRight,
        CameraCarLookUp,
        CameraCarLookBack,
        CameraToggle,
        feBack,
        feMoveUp,
        feMoveDown,
        feMoveLeft,
        feMoveRight,
        feSelect,
        feFunction1,
        feFunction2,
        feMouseLeft,
        feMouseRight,
        feMouseUp,
        feMouseDown,

        P1_KBD_Start,
        P1_KBD_Gas,
        P1_KBD_Brake,
        P1_KBD_EBrake,
        P1_KBD_Nitro,
        P1_KBD_Left,
        P1_KBD_Right,
        LeftStickX = 200,
        LeftStickY = 201,
        KeyboardEsc = 202
    };

    public enum ActiveState : uint
    {
        ACTIVE_NONE = 0,
        ACTIVE_GAMEPLAY = 1 << 0,
        ACTIVE_FRONTEND = 1 << 1,
        ACTIVE_SS_GAME = 1 << 2,
        ACTIVE_FIRST_PERSON = 1 << 3,
        ACTIVE_ANIM_CAM = 1 << 4,
        DEACTIVE_ANIM_CAM = 0xfffffffe,
        ACTIVE_ALL = 0xffffffff
    }

    public enum MapType
    {
        Character,
        Vehicle,
        Frontend
    }

    public static MapType ButtonToMapType(Buttons button)
    {
        switch (button)
        {
            case Buttons.MoveUp:
            case Buttons.MoveDown:
            case Buttons.MoveLeft:
            case Buttons.MoveRight:
            case Buttons.Attack:
            case Buttons.Jump:
            case Buttons.Sprint:
            case Buttons.DoAction:
            case Buttons.CameraLeft:
            case Buttons.CameraRight:
            case Buttons.CameraMoveIn:
            case Buttons.CameraMoveOut:
            case Buttons.CameraZoom:
            case Buttons.CameraLookUp:
                return MapType.Character;
            case Buttons.Accelerate:
            case Buttons.Reverse:
            case Buttons.SteerLeft:
            case Buttons.SteerRight:
            case Buttons.GetOutCar:
            case Buttons.HandBrake:
            case Buttons.Horn:
            case Buttons.ResetCar:
            case Buttons.CameraCarLeft:
            case Buttons.CameraCarRight:
            case Buttons.CameraCarLookUp:
            case Buttons.CameraCarLookBack:
            case Buttons.CameraToggle:
                return MapType.Vehicle;
            case Buttons.feBack:
            case Buttons.feMoveUp:
            case Buttons.feMoveDown:
            case Buttons.feMoveLeft:
            case Buttons.feMoveRight:
            case Buttons.feSelect:
            case Buttons.feFunction1:
            case Buttons.feFunction2:
            case Buttons.feMouseLeft:
            case Buttons.feMouseRight:
            case Buttons.feMouseUp:
            case Buttons.feMouseDown:
                return MapType.Frontend;
            default:
                {
                    if (button >= Buttons.P1_KBD_Start && button <= Buttons.P1_KBD_Right)
                        return MapType.Frontend;

                    return MapType.Character;
                }
        }
    }

    public static readonly Dictionary<ulong, Buttons> KeyToButtonMap = [];
    static InputManager()
    {
        foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            KeyToButtonMap[Helpers.radMakeKey(button.ToString())] = button;
    }

    public InputManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IRadControllerConnectionChangeCallbackVFTableOffset = 0;

    internal const uint IControllerSystem2Offset = IRadControllerConnectionChangeCallbackVFTableOffset + sizeof(uint);

    internal const uint GameDataHandlerVFTableOffset = IControllerSystem2Offset + sizeof(uint);

    internal const uint ControllerArrayOffset = GameDataHandlerVFTableOffset + sizeof(uint);
    public ClassArray<UserController> ControllerArray => new(Memory, Address + ControllerArrayOffset, UserController.Size, MAX_CONTROLLERS);

    internal const uint GameStateOffset = ControllerArrayOffset + UserController.Size * MAX_CONTROLLERS;
    public ActiveState GameState
    {
        get => (ActiveState)ReadUInt32(GameStateOffset);
        set => WriteUInt32(GameStateOffset, (uint)value);
    }

    internal const uint Bitfield1Offset = GameStateOffset + sizeof(uint);
    private byte Bitfield1
    {
        get => ReadByte(Bitfield1Offset);
        set => WriteByte(Bitfield1Offset, value);
    }

    public bool ChangeGameState
    {
        get => (Bitfield1 & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield1 |= 0b00000001;
            else
                Bitfield1 &= 0b11111110;
        }
    }

    public bool ConnectStateChanged
    {
        get => (Bitfield1 & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield1 |= 0b00000010;
            else
                Bitfield1 &= 0b11111101;
        }
    }

    public bool IsRumbleEnabled
    {
        get => (Bitfield1 & 0b00000100) != 0;
        set
        {
            if (value)
                Bitfield1 |= 0b00000100;
            else
                Bitfield1 &= 0b11111011;
        }
    }

    public bool IsResetting
    {
        get => (Bitfield1 & 0b00001000) != 0;
        set
        {
            if (value)
                Bitfield1 |= 0b00001000;
            else
                Bitfield1 &= 0b11110111;
        }
    }

    public bool ResetEnabled
    {
        get => (Bitfield1 & 0b00010000) != 0;
        set
        {
            if (value)
                Bitfield1 |= 0b00010000;
            else
                Bitfield1 &= 0b11101111;
        }
    }

    internal const uint RegisteredControllerIDOffset = Bitfield1Offset + sizeof(uint);
    public StructArray<int> RegisteredControllerID => new(Memory, ReadUInt32(RegisteredControllerIDOffset), sizeof(int), MAX_PLAYERS);

    internal const uint ResetTimeoutOffset = RegisteredControllerIDOffset + sizeof(int) * MAX_PLAYERS;
    public uint ResetTimeout
    {
        get => ReadUInt32(ResetTimeoutOffset);
        set => WriteUInt32(ResetTimeoutOffset, value);
    }

    internal const uint IsProScanButtonsPressedOffset = ResetTimeoutOffset + sizeof(uint);
    public bool IsProScanButtonsPressed
    {
        get => ReadBoolean(IsProScanButtonsPressedOffset);
        set => WriteBoolean(IsProScanButtonsPressedOffset, value);
    }

    internal const uint FEMouseOffset = IsProScanButtonsPressedOffset + 4;
    public SHARMemory.Memory.Class FEMouse => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(FEMouseOffset));
}
