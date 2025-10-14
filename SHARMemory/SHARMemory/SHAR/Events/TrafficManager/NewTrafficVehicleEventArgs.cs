using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.TrafficManager;
public class NewTrafficVehicleEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }

    public NewTrafficVehicleEventArgs(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}
