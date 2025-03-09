using SHARMemory.SHAR.Classes;
using System;
using System.Runtime.InteropServices;

namespace SHARMemory.SHAR;
/// <summary>
/// Handles all of SHAR's custom fuinctions.
/// </summary>
public sealed class Functions
{
    private readonly Memory _memory;

    internal Functions(Memory memory)
    {
        _memory = memory;
    }

    /*
    struct GetMerchandiseParams
    {
        void* Func;
        void* RewardManager;
        int Level;
        int Index;
    };
    */
    [StructLayout(LayoutKind.Sequential)]
    private struct GetMerchandiseParams
    {
        public IntPtr Func;
        public IntPtr RewardManager;
        public uint Level;
        public uint Index;
    }

    /*
    unsigned int WINAPI GetMerchandise(const GetMerchandiseParams & params)
    {
        void* Func = params.Func;
        void* RewardManager = params.RewardManager;
        int Level = params.Level;
        int Index = params.Index;
        __asm
        {
            mov eax, Level
            mov edx, RewardManager
            mov ecx, Index
            call Func
        }
    }
    */
    private readonly byte[] GetMerchandiseBytes = [0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x0C, 0x8B, 0x4D, 0x08, 0x8B, 0x01, 0x89, 0x45, 0xF4, 0x8B, 0x41, 0x04, 0x89, 0x45, 0xFC, 0x8B, 0x41, 0x08, 0x89, 0x45, 0x08, 0x8B, 0x41, 0x0C, 0x89, 0x45, 0xF8, 0x8B, 0x45, 0x08, 0x8B, 0x55, 0xFC, 0x8B, 0x4D, 0xF8, 0xFF, 0x55, 0xF4, 0x8B, 0xE5, 0x5D, 0xC2, 0x04, 0x00];
    private IntPtr GetMerchandiseAddress = IntPtr.Zero;
    /// <summary>
    /// Uses SHAR's <c>RewardsManager::GetMerchandise</c> function to get a <see cref="Merchandise"/> instance.
    /// </summary>
    /// <param name="level">The level of the merchandise.</param>
    /// <param name="index">The index of the merchandise.</param>
    /// <returns>The Merchandise.</returns>
    public Merchandise GetMerchandise(uint level, uint index)
    {
        if (GetMerchandiseAddress == IntPtr.Zero)
            GetMerchandiseAddress = _memory.InjectFunction(GetMerchandiseBytes);

        GetMerchandiseParams parameter = new()
        {
            Func = (IntPtr)_memory.SelectAddress(0x4622E0, 0x4623B0, 0x462020, 0x461E20),
            RewardManager = (IntPtr)_memory.Singletons.RewardsManager.Address,
            Level = level,
            Index = index
        };

        var address = _memory.Execute(GetMerchandiseAddress, parameter);
        return _memory.ClassFactory.Create<Merchandise>(address);
    }
    /// <summary>
    /// Uses SHAR's <c>RewardsManager::GetMerchandise</c> function to get a <see cref="Merchandise"/> instance.
    /// </summary>
    /// <param name="level">The level of the merchandise.</param>
    /// <param name="index">The index of the merchandise.</param>
    /// <returns>The Merchandise.</returns>
    public Merchandise GetMerchandise(int level, int index) => GetMerchandise((uint)level, (uint)index);

    /*
    struct TriggerEventParams
    {
    void* EventManagerGetInstanceFunc;
    void* EventManagerTriggerEventFunc;
    int Event;
    void* Param;
    };
    */
    [StructLayout(LayoutKind.Sequential)]
    private struct TriggerEventParams
    {
        public IntPtr EventManagerGetInstanceFunc;
        public IntPtr EventManagerTriggerEventFunc;
        public int Event;
        public IntPtr Param;
    }

    /*
    unsigned int WINAPI TriggerEvent(const TriggerEventParams& params)
    {
        void* EventManagerGetInstanceFunc = params.EventManagerGetInstanceFunc;
        void* EventManagerTriggerEventFunc = params.EventManagerTriggerEventFunc;
        int Event = params.Event;
        void* Param = params.Param;
        __asm
        {
            push Param
            push Event
            call EventManagerGetInstanceFunc
            mov edx, eax
            call EventManagerTriggerEventFunc
        }
    }
    */
    private readonly byte[] TriggerEventBytes = [0x55, 0x8b, 0xec, 0x83, 0xec, 0x0c, 0x8b, 0x4d, 0x08, 0x8b, 0x01, 0x89, 0x45, 0xf8, 0x8b, 0x41, 0x04, 0x89, 0x45, 0xf4, 0x8b, 0x41, 0x08, 0x89, 0x45, 0xfc, 0x8b, 0x41, 0x0c, 0x89, 0x45, 0x08, 0xff, 0x75, 0x08, 0xff, 0x75, 0xfc, 0xff, 0x55, 0xf8, 0x8b, 0xd0, 0xff, 0x55, 0xf4, 0x8b, 0xe5, 0x5d, 0xc2, 0x04, 0x00];
    private IntPtr TriggerEventAddress = IntPtr.Zero;
    /// <summary>
    /// Uses SHAR's <c>EventManager::TriggerEvent</c> function to trigger an event.
    /// </summary>
    /// <param name="event">The event to trigger.</param>
    /// <param name="param">The index of the merchandise.</param>
    /// <returns>The number of listeners notified.</returns>
    public uint TriggerEvent(Globals.Events @event, uint param = 0)
    {
        if (TriggerEventAddress == IntPtr.Zero)
            TriggerEventAddress = _memory.InjectFunction(TriggerEventBytes);

        TriggerEventParams parameter = new()
        {
            EventManagerGetInstanceFunc = (IntPtr)_memory.SelectAddress(0x4329A0, 0, 0, 0),
            EventManagerTriggerEventFunc = (IntPtr)_memory.SelectAddress(0x432AD0, 0, 0, 0),
            Event = (int)@event,
            Param = (IntPtr)param
        };

        return _memory.Execute(TriggerEventAddress, parameter);
    }

    /*
    struct LookupStringParams
    {
        void* Func;
        char* Name;
    };
    */
    [StructLayout(LayoutKind.Sequential)]
    private struct LookupStringParams
    {
        public IntPtr Func;
        public string Name;
    }

    /*
    unsigned int WINAPI LookupString(const LookupStringParams & params)
    {
        void* Func = params.Func;
        char* Name = params.Name;
        __asm
        {
            mov edx, Name
            call Func
        }
    }
    */
    private readonly byte[] LookupStringBytes = [0x55, 0x8B, 0xEC, 0x51, 0x8B, 0x4D, 0x08, 0x8B, 0x01, 0x89, 0x45, 0xFC, 0x8B, 0x41, 0x04, 0x89, 0x45, 0x08, 0x8B, 0x55, 0x08, 0xFF, 0x55, 0xFC, 0x8B, 0xE5, 0x5D, 0xC2, 0x04, 0x00, 0xCC, 0xCC];
    private IntPtr LookupStringAddress = IntPtr.Zero;
    /// <summary>
    /// Uses SHAR's <see cref="FeTextBible"/> to lookup a string's value.
    /// </summary>
    /// <param name="name">The name of the string to lookup from text bible.</param>
    /// <returns>The string value or <c>null</c>.</returns>
    public string LookupString(string name)
    {
        if (LookupStringAddress == IntPtr.Zero)
            LookupStringAddress = _memory.InjectFunction(LookupStringBytes);

        LookupStringParams parameter = new()
        {
            Func = (IntPtr)_memory.SelectAddress(0x46E570, 0x46E600, 0x46E220, 0x46E000),
            Name = name
        };

        var address = _memory.Execute(LookupStringAddress, parameter);
        return address == 0 ? null : _memory.ReadString(address, System.Text.Encoding.Unicode);
    }
}
