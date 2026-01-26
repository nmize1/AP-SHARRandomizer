using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.ParkedCarManager;
public class NewParkedCarEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }

    public NewParkedCarEventArgs(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}
