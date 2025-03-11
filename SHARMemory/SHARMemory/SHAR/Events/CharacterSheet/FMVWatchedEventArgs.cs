using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class FMVWatchedEventArgs : EventArgs
{
    public int Level { get; }

    public FMVWatchedEventArgs(int level)
    {
        Level = level;
    }
}