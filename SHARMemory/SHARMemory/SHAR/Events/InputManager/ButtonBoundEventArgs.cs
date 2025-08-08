using System;

namespace SHARMemory.SHAR.Events.InputManager;
public class ButtonBoundEventArgs : EventArgs
{
    public Classes.InputManager.Buttons Button { get; }
    public ControllerButtonMapping OldMapping { get; }
    public ControllerButtonMapping NewMapping { get; }

    public ButtonBoundEventArgs(Classes.InputManager.Buttons button, ControllerButtonMapping oldMapping, ControllerButtonMapping newMapping)
    {
        Button = button;
        OldMapping = oldMapping;
        NewMapping = newMapping;
    }
}
