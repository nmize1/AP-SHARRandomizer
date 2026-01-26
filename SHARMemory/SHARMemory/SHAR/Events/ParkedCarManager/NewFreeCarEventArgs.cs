using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.ParkedCarManager;
public class NewFreeCarEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }

    public NewFreeCarEventArgs(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}
