using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.RewardsManager;

public class RewardUnlockedEventArgs : EventArgs
{
    public enum RewardType
    {
        StreetRace,
        BonusMission,
        Cards,
        DefaultCar,
        DefaultSkin,
        GoldCards,
    }

    public int Level { get; }
    public RewardType Type { get; }
    public Reward Reward { get; }

    public RewardUnlockedEventArgs(int level, RewardType type, Reward reward)
    {
        Level = level;
        Type = type;
        Reward = reward;
    }
}