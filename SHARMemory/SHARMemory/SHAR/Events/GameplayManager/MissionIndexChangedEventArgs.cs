using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public class MissionIndexChangedEventArgs : EventArgs
{
    public Globals.RenderEnums.LevelEnum? LastLevel { get; }
    public Mission? LastMission { get; }
    public Globals.RenderEnums.LevelEnum? NewLevel { get; }
    public Mission? NewMission { get; }

    public MissionIndexChangedEventArgs(Globals.RenderEnums.LevelEnum? lastLevel, int lastMissionIndex, Globals.RenderEnums.LevelEnum? newLevel, int newMissionIndex)
    {
        LastLevel = lastLevel;
        LastMission = lastMissionIndex >= 0 ? (Mission)lastMissionIndex : null;
        NewLevel = newLevel;
        NewMission = newMissionIndex >= 0 ? (Mission)newMissionIndex : null;
    }
}