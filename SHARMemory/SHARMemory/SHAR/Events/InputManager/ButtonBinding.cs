using SHARMemory.SHAR.Classes;

namespace SHARMemory.SHAR.Events.InputManager;
public class ButtonBinding
{
    public RealController.ControllerTypes ControllerType { get; }
    public int ButtonId { get; }
    public RealController.DirectionType Direction { get; }

    public ButtonBinding(RealController.ControllerTypes controllerType, int buttonId, RealController.DirectionType direction)
    {
        ControllerType = controllerType;
        ButtonId = buttonId;
        Direction = direction;
    }

    public static ButtonBinding FromButton(int[] buttonMap, Classes.InputManager.Buttons button)
    {
        if (buttonMap == null)
            return null;

        var iButton = (int)button;
        if (iButton < 0 || iButton > buttonMap.Length)
            return null;

        var code = buttonMap[iButton];
        if (code == -1)
            return null;

        var buttonId = code & 0xFFFFFF;
        var controllerType = (RealController.ControllerTypes)(code >> 0x1C);
        var dir = (RealController.DirectionType)((code >> 0x18) & 0xF);

        return new(controllerType, buttonId, dir);
    }

    public override bool Equals(object obj)
    {
        if (obj is ButtonBinding other)
        {
            return ControllerType == other.ControllerType &&
                   ButtonId == other.ButtonId &&
                   Direction == other.Direction;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + ControllerType.GetHashCode();
            hash = hash * 23 + ButtonId.GetHashCode();
            hash = hash * 23 + Direction.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => $"{ControllerType} - {ButtonId} - {Direction}";
}