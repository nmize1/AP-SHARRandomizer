using System;

namespace SHARMemory.SHAR.Events.RewardsManager;

public class MerchandisePurchased : EventArgs
{
    public int Level { get; }
    public int MerchandiseIndex { get; }
    public Classes.Merchandise Merchandise { get; }

    public MerchandisePurchased(int level, int merchandiseIndex, Classes.Merchandise merchandise)
    {
        Level = level;
        MerchandiseIndex = merchandiseIndex;
        Merchandise = merchandise;
    }
}