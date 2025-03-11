using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

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