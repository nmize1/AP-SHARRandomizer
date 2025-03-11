using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public class MissionChangedEventArgs : EventArgs
{
    public Classes.Mission LastMission { get; }
    public Classes.Mission NewMission { get; }
    public Globals.RenderEnums.LevelEnum? Level { get; }
    public Mission? Mission { get; }

    public MissionChangedEventArgs(Classes.Mission lastMission, Classes.Mission newMission, Globals.RenderEnums.LevelEnum? level, int missionIndex)
    {
        LastMission = lastMission;
        NewMission = newMission;
        Level = level;
        Mission = missionIndex >= 0 ? (Mission)missionIndex : null;
    }
}