using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class WaspDestroyedEventArgs : EventArgs
{
    public int Level { get; }
    public int Count { get; }

    public WaspDestroyedEventArgs(int level, int count)
    {
        Level = level;
        Count = count;
    }
}