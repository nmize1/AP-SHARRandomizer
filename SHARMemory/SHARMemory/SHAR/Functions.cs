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
}
