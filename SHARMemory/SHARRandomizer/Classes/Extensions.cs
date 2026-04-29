using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SHARRandomizer.Classes
{
    public static class Extensions
    {
        public static bool InGame(this Memory memory) => memory.IsRunning && memory.Singletons.GameFlow?.NextContext switch
        {
            GameFlow.GameState.NormalInGame or GameFlow.GameState.DemoInGame or GameFlow.GameState.BonusInGame => true,
            _ => false,
        };

        public static InterprocessCommunication GetInterprocessCommunication(SHARMemory.SHAR.Memory memory) => new(memory.Process.Id);

        public static void Teleport(Memory memory, System.Numerics.Vector3 pos, float rotationY)
        {
            using var ipc = GetInterprocessCommunication(memory);
            IPCUtils.Teleport(ipc, memory, pos, true, rotationY);
        }

        public static void Repair(Memory memory)
        {
            using var ipc = GetInterprocessCommunication(memory);
            IPCUtils.Repair(ipc, memory);
        }

        public static void Dynaload(Memory memory, string dynString)
        {
            using var ipc = GetInterprocessCommunication(memory);
            IPCUtils.Dynaload(ipc, memory, dynString);
        }

        public static void SetString(FeLanguage tb, string key, string str)
        {
            try
            {
                tb.SetString(key, str);
            }
            catch
            {
                var len = tb.GetString(key).Length;

                if (len <= 0)
                {
                    tb.SetString(key, ""); //the hell?
                }
                else if (len < 3)
                {
                    tb.SetString(key, ".");
                }
                else
                {
                    tb.SetString(key, str.Substring(0, len - 3) + "...");
                }
            }
        }
    }
}
