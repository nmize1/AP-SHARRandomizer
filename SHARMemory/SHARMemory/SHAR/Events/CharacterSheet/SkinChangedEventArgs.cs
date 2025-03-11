using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class SkinChangedEventArgs : EventArgs
{
    public int Level { get; }
    public string LastSkin { get; }
    public string NewSkin { get; }

    public SkinChangedEventArgs(int level, string lastSkin, string newSkin)
    {
        Level = level;
        LastSkin = lastSkin;
        NewSkin = newSkin;
    }
}