using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public class MissionStageChangedEventArgs : EventArgs
{
    public MissionStage LastStage { get; }
    public MissionStage NewStage { get; }
    public Globals.RenderEnums.LevelEnum? Level { get; }
    public Mission? Mission { get; }

    public MissionStageChangedEventArgs(MissionStage lastStage, MissionStage newStage, Globals.RenderEnums.LevelEnum? level, int missionIndex)
    {
        LastStage = lastStage;
        NewStage = newStage;
        Level = level;
        Mission = missionIndex >= 0 ? (Mission)missionIndex : null;
    }
}