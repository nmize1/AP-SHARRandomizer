using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.TrafficManager;
public class TrafficVehicleCreatedEventArgs : EventArgs
{
    public TrafficVehicle TrafficVehicle { get; }

    public TrafficVehicleCreatedEventArgs(TrafficVehicle trafficVehicle)
    {
        TrafficVehicle = trafficVehicle;
    }
}
