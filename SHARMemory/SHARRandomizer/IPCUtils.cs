using SHARMemory.SHAR.Classes;
using SHARRandomizer.Classes;
using System.Numerics;
using System.Text;
using static SHARMemory.SHAR.Classes.GameFlow;

namespace SHARRandomizer;

public static class IPCUtils
{
    private static readonly HashSet<GameState> InGameStates = [
        GameState.NormalInGame,
        GameState.NormalPaused,
        GameState.BonusInGame,
        GameState.DemoInGame,
    ];

    public static void Teleport(InterprocessCommunication interprocessCommunication, SHARMemory.SHAR.Memory sharMemory, Vector3 pos, bool bottom, float rotationY)
    {
        var currentContext = sharMemory.Singletons.GameFlow.CurrentContext;
        if (!InGameStates.Contains(currentContext))
            throw new Exception("You don't appear to be in game.");

        if (sharMemory.Singletons.CharacterManager is not CharacterManager characterManager || characterManager.Player is not Character player)
            throw new Exception("Could not find your player character.");

        interprocessCommunication.CheckVersionAndHack("1.17.1", "DebugCommunication");
        interprocessCommunication.Send("DebugCommunication");

        if (player.Car is Vehicle car)
        {
            interprocessCommunication.Writer.Write((uint)InterprocessCommunication.DebugCommunicationType.CarTeleport);
            interprocessCommunication.Writer.Write(car.Address);
            if (bottom)
            {
                var wheelRadius = car.Wheels[0].Radius;
                var suspensionRestPoint = car.SuspensionRestPoints[0];
                pos.Y += wheelRadius - suspensionRestPoint.Y;
            }
            var mat = Matrix4x4.CreateRotationY(rotationY) * Matrix4x4.CreateTranslation(pos);
            interprocessCommunication.Writer.Write(mat);
        }
        else
        {
            interprocessCommunication.Writer.Write((uint)InterprocessCommunication.DebugCommunicationType.CharacterTeleport);
            interprocessCommunication.Writer.Write(player.Address);
            interprocessCommunication.Writer.Write(pos);
            interprocessCommunication.Writer.Write(rotationY);
            interprocessCommunication.Writer.Write((byte)(1 | 2));
        }
    }

    public static void Repair(InterprocessCommunication interprocessCommunication, SHARMemory.SHAR.Memory sharMemory)
    {
        var currentContext = sharMemory.Singletons.GameFlow.CurrentContext;
        if (!InGameStates.Contains(currentContext))
            throw new Exception("You don't appear to be in game.");

        if (sharMemory.Singletons.CharacterManager is not CharacterManager characterManager || characterManager.Player is not Character player)
            throw new Exception("Could not find your player character.");

        interprocessCommunication.CheckVersionAndHack("1.17.1", "DebugCommunication");
        interprocessCommunication.Send("DebugCommunication");


        interprocessCommunication.Writer.Write((uint)InterprocessCommunication.DebugCommunicationType.TriggerEvent);
        interprocessCommunication.Writer.Write((uint)SHARMemory.SHAR.Globals.Events.REPAIR_CAR);
        interprocessCommunication.Writer.Write((uint)0);
    }

    public static void Dynaload(InterprocessCommunication interprocessCommunication, SHARMemory.SHAR.Memory sharMemory, string dynString)
    {
        var currentContext = sharMemory.Singletons.GameFlow.CurrentContext;
        if (!InGameStates.Contains(currentContext))
            throw new Exception("You don't appear to be in game.");

        interprocessCommunication.CheckVersionAndHack("1.17.1", "DebugCommunication");
        interprocessCommunication.Send("DebugCommunication");

        interprocessCommunication.Writer.Write((uint)InterprocessCommunication.DebugCommunicationType.DynaLoadDataSet);
        interprocessCommunication.Writer.Write((uint)dynString.Length);
        interprocessCommunication.Writer.Write(Encoding.ASCII.GetBytes(dynString));
    }
}
