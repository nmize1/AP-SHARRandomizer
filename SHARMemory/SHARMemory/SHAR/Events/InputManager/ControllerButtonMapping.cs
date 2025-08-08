using System.Collections.Generic;
using System.Linq;

namespace SHARMemory.SHAR.Events.InputManager;

public class ControllerButtonMapping
{
    public int ControllerIndex { get; }
    public Classes.InputManager.Buttons Button { get; }
    public List<ButtonBinding> Bindings { get; }

    public ControllerButtonMapping(int controllerIndex, Classes.InputManager.Buttons button, List<ButtonBinding> bindings)
    {
        ControllerIndex = controllerIndex;
        Button = button;
        Bindings = bindings;
    }

    public override bool Equals(object obj)
    {
        if (obj is ControllerButtonMapping other)
        {
            return ControllerIndex == other.ControllerIndex &&
                   Button == other.Button &&
                   Bindings.SequenceEqual(other.Bindings);
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + ControllerIndex.GetHashCode();
            hash = hash * 23 + Button.GetHashCode();
            foreach (var b in Bindings)
                hash = hash * 23 + (b?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override string ToString() => $"{Button} | {string.Join(" | ", Bindings)}";
}
