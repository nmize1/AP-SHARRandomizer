using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.RewardsManager;

public class MerchandisePurchasedEventArgs : EventArgs
{
    public int Level { get; }
    public int MerchandiseIndex { get; }
    public Merchandise Merchandise { get; }

    public MerchandisePurchasedEventArgs(int level, int merchandiseIndex, Merchandise merchandise)
    {
        Level = level;
        MerchandiseIndex = merchandiseIndex;
        Merchandise = merchandise;
    }
}