using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class StreetRaceCompleteEventArgs : EventArgs
{
    public int Level { get; }
    public int Race { get; }

    public StreetRaceCompleteEventArgs(int level, int race)
    {
        Level = level;
        Race = race;
    }
}