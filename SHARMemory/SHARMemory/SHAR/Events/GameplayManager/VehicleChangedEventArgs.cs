using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.GameplayManager;

public class VehicleChangedEventArgs : EventArgs
{
    public Vehicle LastVehicle { get; }
    public Vehicle NewVehicle { get; }

    public VehicleChangedEventArgs(Vehicle lastVehicle, Vehicle newVehicle)
    {
        LastVehicle = lastVehicle;
        NewVehicle = newVehicle;
    }
}