using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class GagViewedEventArgs : EventArgs
{
    public int Level { get; }
    public int Gag { get; }

    public GagViewedEventArgs(int level, int gag)
    {
        Level = level;
        Gag = gag;
    }
}