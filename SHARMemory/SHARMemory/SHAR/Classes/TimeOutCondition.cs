using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;
using System.Collections.Generic;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTimeOutCondition@@")]
public class TimeOutCondition : MissionCondition
{
    public TimeOutCondition(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint DoneOffset = LeaveInteriorOffset + 3; // Padding
    public bool Done
    {
        get => ReadBoolean(DoneOffset);
        set => WriteBoolean(DoneOffset, value);
    }

    public const uint Size = DoneOffset + 4;

    public static TimeOutCondition CreateNew(Memory memory, bool isViolated, bool leaveInterior, bool done)
    {
        List<byte> classBytes = new((int)Size);

        classBytes.AddRange(BitConverter.GetBytes(memory.SelectAddress(0x611288, 0x611330, 0x6112F8, 0x611388)));
        classBytes.AddRange(BitConverter.GetBytes((int)ConditionTypes.TimeOut));
        classBytes.Add((byte)(isViolated ? 1 : 0));
        classBytes.Add((byte)(leaveInterior ? 1 : 0));
        classBytes.Add(0);
        classBytes.Add(0);
        classBytes.Add((byte)(done ? 1 : 0));
        classBytes.Add(0);
        classBytes.Add(0);
        classBytes.Add(0);

        uint address = memory.AllocateAndWriteMemory([.. classBytes]);
        return memory.ClassFactory.Create<TimeOutCondition>(address);
    }
}
