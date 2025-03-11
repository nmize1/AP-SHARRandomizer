using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

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