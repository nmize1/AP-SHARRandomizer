using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class BonusMissionCompleteEventArgs : EventArgs
{
    public int Level { get; }

    public BonusMissionCompleteEventArgs(int level)
    {
        Level = level;
    }
}