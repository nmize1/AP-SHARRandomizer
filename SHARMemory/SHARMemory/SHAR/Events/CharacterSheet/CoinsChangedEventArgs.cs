using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

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