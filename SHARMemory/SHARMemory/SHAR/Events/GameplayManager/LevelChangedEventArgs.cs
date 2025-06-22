using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public class LevelChangedEventArgs : EventArgs
{
    public Globals.RenderEnums.LevelEnum? LastLevel { get; }
    public Globals.RenderEnums.LevelEnum? NewLevel { get; }

    public LevelChangedEventArgs(Globals.RenderEnums.LevelEnum? lastLevel, Globals.RenderEnums.LevelEnum? newLevel)
    {
        LastLevel = lastLevel;
        NewLevel = newLevel;
    }
}