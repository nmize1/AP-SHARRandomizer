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
    public Classes.CharacterSheet.PersistentObjectStateSector Sector { get; }
    public uint Index { get; }

    public PersistentObjectDestroyedEventArts(uint index)
    {
        Sector = (Classes.CharacterSheet.PersistentObjectStateSector)(index / 128);
        Index = index % 128;
    }
}