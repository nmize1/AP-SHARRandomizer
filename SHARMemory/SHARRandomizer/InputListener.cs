using SHARMemory.SHAR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHARRandomizer
{
    [Flags]
    public enum XInputButtons : ushort
    {
        DPadUp = 0x0001,
        DPadDown = 0x0002,
        DPadLeft = 0x0004,
        DPadRight = 0x0008,
        Start = 0x0010,
        Back = 0x0020,
        LeftThumb = 0x0040,
        RightThumb = 0x0080,
        LeftShoulder = 0x0100,
        RightShoulder = 0x0200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000
    }

    class XInput
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [DllImport("XInput1_4.dll", EntryPoint = "XInputGetState")]
        private static extern int XInputGetState(uint dwUserIndex, out XINPUT_STATE pState);

        public static bool GetControllerState(int index, out XINPUT_STATE state)
        {
            return XInputGetState((uint)index, out state) == 0;
        }
    }

    public class InputListener
    {
        public Memory memory;

        public event EventHandler<ButtonEventArgs>? ButtonUp;
        public event EventHandler<ButtonEventArgs>? ButtonDown;

        private bool _listening = false;

        public void Stop() => _listening = false;

        public void Start()
        {
            if (_listening) return;

            Console.WriteLine("Listening for controller input...");

            _listening = true;
            Task.Run(ListenXInput);
            Task.Run(ListenKeyboard);
        }

        private void ListenXInput()
        {
            var states = new Dictionary<XInputButtons, bool>();
            foreach (XInputButtons button in Enum.GetValues(typeof(XInputButtons)))
                states[button] = false;

            while (_listening)
            {
                if (!XInput.GetControllerState(0, out var state))
                    continue;

                var buttons = state.Gamepad.wButtons;
                foreach (XInputButtons button in Enum.GetValues(typeof(XInputButtons)))
                {
                    var pressed = (buttons & (ushort)button) != 0;

                    if (pressed != states[button])
                    {
                        states[button] = pressed;

                        if (pressed)
                            ButtonDown?.Invoke(this, new(button));
                        else
                            ButtonUp?.Invoke(this, new(button));
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private void ListenKeyboard()
        {
            var keyStates = new Dictionary<ConsoleKey, bool>();
            var numberKeys = new List<ConsoleKey>
            {
                ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4,
                ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9
            };

            foreach (var key in numberKeys)
                keyStates[key] = false;

            while (_listening)
            {
                foreach (var key in numberKeys)
                {
                    bool pressed = (GetAsyncKeyState((int)key) & 0x8000) != 0;

                    if (pressed != keyStates[key])
                    {
                        keyStates[key] = pressed;

                        if (pressed)
                            ButtonDown?.Invoke(this, new ButtonEventArgs(key));
                        else
                            ButtonUp?.Invoke(this, new ButtonEventArgs(key));
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }


        public class ButtonEventArgs : EventArgs
        {
            public Object Button { get; }

            public ButtonEventArgs(Object button)
            {
                Button = button;
            }
        }
    }
}
