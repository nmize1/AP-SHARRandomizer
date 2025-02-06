using System;


namespace SHARMemory.SHAR.Events.CharacterSheet;

public enum Sector
{
    L1R1,
    L1R2,
    L1R3,
    L1R4A,
    L1R4B,
    L1R6,
    L1R7,
    L1Z1,
    L1Z2,
    L1Z3,
    L1Z4,
    L1Z6,
    L1Z7,
    L2R1,
    L2R2,
    L2R3,
    L2R4,
    L2Z1,
    L2Z2,
    L2Z3,
    L2Z4,
    L3R1,
    L3R2,
    L3R3,
    L3R4,
    L3R5,
    L3Z1,
    L3Z2,
    L3Z3,
    L3Z4,
    L3Z5,
    L4R1,
    L4R2,
    L4R3,
    L4R4A,
    L4R4B,
    L4R6,
    L4R7,
    L4Z1,
    L4Z2,
    L4Z3,
    L4Z4,
    L4Z6,
    L4Z7,
    L5R1,
    L5R2,
    L5R3,
    L5R4,
    L5Z1,
    L5Z2,
    L5Z3,
    L5Z4,
    L6R1,
    L6R2,
    L6R3,
    L6R4,
    L6R5,
    L6Z1,
    L6Z2,
    L6Z3,
    L6Z4,
    L6Z5,
    L7R1,
    L7R2,
    L7R3,
    L7R4A,
    L7R4B,
    L7R6,
    L7R7,
    L7Z1,
    L7Z2,
    L7Z3,
    L7Z4,
    L7Z6,
    L7Z7,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
}

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

public class BonusMissionCompleteEventArgs : EventArgs
{
    public int Level { get; }

    public BonusMissionCompleteEventArgs(int level)
    {
        Level = level;
    }
}

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

public class GambleRaceCompleteEventArgs : EventArgs
{
    public int Level { get; }

    public GambleRaceCompleteEventArgs(int level)
    {
        Level = level;
    }
}

public class WaspDestroyedEventArgs : EventArgs
{
    public int Level { get; }
    public int Count { get; }

    public WaspDestroyedEventArgs(int level, int count)
    {
        Level = level;
        Count = count;
    }
}

public class FMVWatchedEventArgs : EventArgs
{
    public int Level { get; }

    public FMVWatchedEventArgs(int level)
    {
        Level = level;
    }
}

public class CarPurchasedEventArgs : EventArgs
{
    public int Level { get; }
    public int Count { get; }

    public CarPurchasedEventArgs(int level, int count)
    {
        Level = level;
        Count = count;
    }
}

public class SkinPurchasedEventArgs : EventArgs
{
    public int Level { get; }
    public int Count { get; }

    public SkinPurchasedEventArgs(int level, int count)
    {
        Level = level;
        Count = count;
    }
}

public class SkinChangedEventArgs : EventArgs
{
    public int Level { get; }
    public string LastSkin { get; }
    public string NewSkin { get; }

    public SkinChangedEventArgs(int level, string lastSkin, string newSkin)
    {
        Level = level;
        LastSkin = lastSkin;
        NewSkin = newSkin;
    }
}

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

public class CoinsChangedEventArgs : EventArgs
{
    public int LastCoins { get; }
    public int NewCoins { get; }

    public CoinsChangedEventArgs(int lastCoins, int newCoins)
    {
        LastCoins = lastCoins;
        NewCoins = newCoins;
    }
}

public class PersistentObjectDestroyedEventArts : EventArgs
{
    public Sector Sector { get; }
    public uint Index { get; }

    public PersistentObjectDestroyedEventArts(uint index)
    {
        Sector = (Sector)(index / 128);
        Index = index % 128;
    }
}