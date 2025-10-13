using SHARMemory.Memory;

namespace SHARMemory.SHAR;

/// <summary>
/// A static class to add SHAR specific functions to <see cref="PointerArray{T}"/>.
/// </summary>
public static class PointerArrayExtensions
{
    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from an <c>IPtrArray</c> in SHAR.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="ProcessMemory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The address offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromPtrArray<T>(ProcessMemory memory, Class @class, uint offset) where T : Class => new(memory, @class.ReadUInt32(offset + 8), (int)@class.ReadUInt32(offset + 4));

    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from a <c>rVector</c> in SHAR.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="SHAR.Memory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The address offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromRVector<T>(ProcessMemory memory, Class @class, uint offset) where T : Class => new(memory, @class.ReadUInt32(offset + 4), (int)@class.ReadUInt32(offset + 8));

    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from a <c>std::vector</c>.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="SHAR.Memory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The address offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromVector<T>(ProcessMemory memory, Class @class, uint offset) where T : Class => new(memory, @class.ReadUInt32(offset), (int)(@class.ReadUInt32(offset + 8) - @class.ReadUInt32(offset)) / 4);

    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from a <c>TList</c> in SHAR.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="SHAR.Memory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The address offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromTList<T>(ProcessMemory memory, Class @class, uint offset) where T : Class => new(memory, @class.ReadUInt32(offset + 16), (int)@class.ReadUInt32(offset + 12));

    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from a <c>TList</c> pointer in SHAR.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="SHAR.Memory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The pointer offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromTListPointer<T>(ProcessMemory memory, Class @class, uint offset) where T : Class
    {
        uint address = @class.ReadUInt32(offset);

        return new(memory, memory.ReadUInt32(address + 16), (int)memory.ReadUInt32(address + 12));
    }

    /// <summary>
    /// Creates a <see cref="PointerArray{T}"/> from a <c>SwapArray</c> in SHAR.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="ProcessMemory"/> to use.
    /// The <see cref="SHAR.Memory"/> to use.
    /// </param>
    /// <param name="class">
    /// The <see cref="Class"/> the array is in.
    /// </param>
    /// <param name="offset">
    /// The address offset in the class.
    /// </param>
    /// <returns>
    /// A new <see cref="PointerArray{T}"/> at the <paramref name="offset"/> in <paramref name="class"/>.
    /// </returns>
    public static PointerArray<T> FromSwapArray<T>(ProcessMemory memory, Class @class, uint offset) where T : Class => new(memory, @class.ReadUInt32(offset + 8), (int)@class.ReadUInt32(offset + 4));
}
