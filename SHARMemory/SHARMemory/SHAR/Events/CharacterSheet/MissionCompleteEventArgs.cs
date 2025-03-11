using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class MissionCompleteEventArgs : EventArgs
{
    public int Level { get; }
    public int Mission { get; }

    public MissionCompleteEventArgs(int level, int mission)
    {
        Level = level;
        Mission = mission;
    }
}