using System;

namespace SHARMemory.SHAR.Events.CardGallery;

public class CardCollectedEventArgs : EventArgs
{
    public int Level { get; }
    public int Card { get; }

    public CardCollectedEventArgs(int level, int card)
    {
        Level = level;
        Card = card;
    }
}