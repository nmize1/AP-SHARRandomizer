using System;
using System.Collections;
using System.Collections.Generic;

namespace SHARMemory.SHAR.Arrays;
/// <summary>
/// An implementation of Radical's SwapArray to be used with class pointers.
/// </summary>
/// <typeparam name="T">The <see cref="Class"/> used in the array.</typeparam>
public class PointerSwapArray<T> : IEnumerable<T> where T : Class
{
    internal const uint MemorySize = sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(uint);

    private readonly Memory Memory;
    /// <summary>
    /// The base address of the SwapArray.
    /// </summary>
    public readonly uint Address;
    /// <summary>
    /// How many elements are allocated for this array.
    /// </summary>
    public int Size => Memory.ReadInt32(Address);
    /// <summary>
    /// How many elements are in this array.
    /// </summary>
    public int UseSize
    {
        get => Memory.ReadInt32(Address + sizeof(int));
        set
        {
            if (value < 0)
                throw new IndexOutOfRangeException($"{nameof(UseSize)} must be equal to or greater than 0.");
            if (value > Size)
                throw new IndexOutOfRangeException($"{nameof(UseSize)} must be less than or equal to {Size}.");

            Memory.WriteInt32(Address + sizeof(int), value);
        }
    }
    /// <summary>
    /// The base address of the underlying array.
    /// </summary>
    public uint ArrayAddress => Memory.ReadUInt32(Address + sizeof(int) + sizeof(int));

    /// <summary>
    /// Get an element of this array.
    /// </summary>
    /// <param name="index">
    /// The index to retrieve. Must be greater than or equal to <c>0</c>, and less than <see cref="UseSize"/>.
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> at <paramref name="index"/>.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if index is out of viable range.
    /// </exception>
    public T this[int index]
    {
        get
        {
            if (index < 0)
                throw new IndexOutOfRangeException($"Index {index} is not equal to or greater than 0.");
            if (index >= UseSize)
                throw new IndexOutOfRangeException($"Index {index} is outside range {UseSize}.");

            return Memory.ClassFactory.Create<T>(Memory.ReadUInt32(ArrayAddress + (uint)index * sizeof(uint)));
        }
        set
        {
            if (index < 0)
                throw new IndexOutOfRangeException($"Index {index} is not equal to or greater than 0.");
            if (index >= UseSize)
                throw new IndexOutOfRangeException($"Index {index} is outside range {UseSize}.");

            Memory.WriteUInt32(ArrayAddress + (uint)index * sizeof(uint), value?.Address ?? 0);
        }
    }

    /// <summary>
    /// The <c>SHARMemory.SHAR.Arrays.PointerSwapArray</c> constructor.
    /// </summary>
    /// <param name="memory">
    /// The <see cref="SHARMemory.SHAR.Memory"/> to use.
    /// </param>
    /// <param name="address">
    /// The base address for the first element in the array.
    /// </param>
    public PointerSwapArray(Memory memory, uint address)
    {
        Memory = memory;
        Address = address;
    }

    /// <summary>
    /// Reads the whole array at once into a <see cref="Action{T}"/>.
    /// </summary>
    /// <returns>
    /// The entire array.
    /// </returns>
    public T[] ToArray()
    {
        byte[] bytes = Memory.ReadBytes(ArrayAddress, sizeof(uint) * (uint)UseSize);

        T[] result = new T[UseSize];
        for (int i = 0; i < UseSize; i++)
            result[i] = Memory.ClassFactory.Create<T>(BitConverter.ToUInt32(bytes, i * sizeof(uint)));

        return result;
    }

    /// <summary>
    /// Writes the whole array at once into a <see cref="Action{T}"/>.
    /// </summary>
    /// <param name="array">
    /// The array to write.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Throws an <see cref="ArgumentException"/> if <paramref name="array"/> length doesn't match <see cref="UseSize"/>.
    /// </exception>
    public void FromArray(T[] array)
    {
        if (array.Length > Size)
            throw new ArgumentException($"{nameof(array)} has a max length of {Size}", nameof(array));

        byte[] bytes = new byte[sizeof(uint) * array.Length];
        for (int i = 0; i < array.Length; i++)
            BitConverter.GetBytes(array[i]?.Address ?? 0).CopyTo(bytes, i * sizeof(uint));

        UseSize = array.Length;
        Memory.WriteBytes(ArrayAddress, bytes);
    }

    private class SwapPointerEnumerator : IEnumerator<T>
    {
        private readonly T[] array;
        private int position = -1;

        public T Current => array[position];
        object IEnumerator.Current => Current;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Causes infinite loop")]
        public SwapPointerEnumerator(PointerSwapArray<T> array) => this.array = array.ToArray();

        public bool MoveNext()
        {
            do
            {
                position++;
                if (position >= array.Length)
                    return false;
            }
            while (Current == null);
            return true;
        }

        public void Reset() => position = -1;

        public void Dispose() { }
    }

    /// <summary>
    /// Gets the <see cref="SwapPointerEnumerator"/> for this array.
    /// </summary>
    /// <returns>
    /// A new enumerator for this array.
    /// </returns>
    public IEnumerator<T> GetEnumerator() => new SwapPointerEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Override <c>ToString</c> to provide a nicer string response.
    /// </summary>
    /// <returns>
    /// The array information
    /// </returns>
    public override string ToString() => $"{typeof(T)}[{UseSize}]";
}
