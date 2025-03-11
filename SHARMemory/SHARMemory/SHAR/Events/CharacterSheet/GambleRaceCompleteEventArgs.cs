using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class GambleRaceCompleteEventArgs : EventArgs
{
    public int Level { get; }

    public GambleRaceCompleteEventArgs(int level)
    {
        Level = level;
    }
}