using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public enum Mission
{
    M1SD,
    M1,
    M2SD,
    M2,
    M3SD,
    M3,
    M4SD,
    M4,
    M5SD,
    M5,
    M6SD,
    M6,
    M7SD,
    M7,
    M8SD,
    M8,
    M9SD,
    M9,
    M10SD,
    M10,
    SR1,
    SR2,
    SR3,
    GR,
    BM,
}

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

public class VehicleChangedEventArgs : EventArgs
{
    public Vehicle LastVehicle { get; }
    public Vehicle NewVehicle { get; }

    public VehicleChangedEventArgs(Vehicle lastVehicle, Vehicle newVehicle)
    {
        LastVehicle = lastVehicle;
        NewVehicle = newVehicle;
    }
}