using System;
using System.Collections;
using System.Collections.Generic;

namespace SHARMemory.SHAR.Arrays;
/// <summary>
/// An implementation of Radical's SwapArray to be used with structs.
/// </summary>
/// <typeparam name="T">The struct used in the array.</typeparam>
public class StructSwapArray<T> : IEnumerable<T> where T : struct
{
    /// <summary>
    /// The base size of the swap array. You must add the size of <typeparamref name="T"/>.
    /// </summary>
    internal const uint BaseSize = sizeof(int) + sizeof(int) + sizeof(uint);

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

    private readonly uint ElementSize;

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

            return Memory.ReadStruct<T>(Address + ElementSize * (uint)index);
        }
        set
        {
            if (index < 0)
                throw new IndexOutOfRangeException($"Index {index} is not equal to or greater than 0.");
            if (index >= UseSize)
                throw new IndexOutOfRangeException($"Index {index} is outside range {UseSize}.");

            Memory.WriteStruct(Address + ElementSize * (uint)index, value);
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
    /// <param name="elementSize">
    /// The size of each element in the array.
    /// </param>
    public StructSwapArray(Memory memory, uint address, uint elementSize)
    {
        Memory = memory;
        Address = address;
        ElementSize = elementSize;
    }

    /// <summary>
    /// Reads the whole array at once into a <see cref="Action{T}"/>.
    /// </summary>
    /// <returns>
    /// The entire array.
    /// </returns>
    public T[] ToArray()
    {
        byte[] bytes = Memory.ReadBytes(ArrayAddress, ElementSize * (uint)UseSize);

        T[] result = new T[UseSize];
        for (int i = 0; i < UseSize; i++)
            result[i] = Memory.StructFromBytes<T>(bytes, i * (int)ElementSize);

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

        byte[] bytes = new byte[ElementSize * array.Length];
        for (int i = 0; i < array.Length; i++)
            Memory.BytesFromStruct<T>(array[i], bytes, i * (int)ElementSize);

        UseSize = array.Length;
        Memory.WriteBytes(ArrayAddress, bytes);
    }

    private class SwapStructEnumerator : IEnumerator<T>
    {
        private readonly T[] array;
        private int position = -1;

        public T Current => array[position];
        object IEnumerator.Current => Current;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Causes infinite loop")]
        public SwapStructEnumerator(StructSwapArray<T> array) => this.array = array.ToArray();

        public bool MoveNext()
        {
            position++;
            return position < array.Length;
        }

        public void Reset() => position = -1;

        public void Dispose() { }
    }

    /// <summary>
    /// Gets the <see cref="SwapStructEnumerator"/> for this array.
    /// </summary>
    /// <returns>
    /// A new enumerator for this array.
    /// </returns>
    public IEnumerator<T> GetEnumerator() => new SwapStructEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Override <c>ToString</c> to provide a nicer string response.
    /// </summary>
    /// <returns>
    /// The array information
    /// </returns>
    public override string ToString() => $"{typeof(T)}[{UseSize}]";
}
