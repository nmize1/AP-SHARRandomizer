using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(TokenStoreInventoryStruct))]
public struct TokenStoreInventory
{
    public const int Size = sizeof(uint) * RewardsManager.MAX_INVENTORY + sizeof(int);

    public Merchandise[] InventoryList;

    public int Counter;

    public TokenStoreInventory(Merchandise[] merchandises, int counter)
    {
        InventoryList = merchandises;
        Counter = counter;
    }

    public override readonly string ToString() => $"{InventoryList} | {Counter}";
}

internal class TokenStoreInventoryStruct : Struct
{
    public override int Size => TokenStoreInventory.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        Merchandise[] Merchandises = new Merchandise[RewardsManager.MAX_INVENTORY];
        for (int i = 0; i < RewardsManager.MAX_INVENTORY; i++)
        {
            Merchandises[i] = Memory.ClassFactory.Create<Merchandise>(BitConverter.ToUInt32(Bytes, Offset));
            Offset += sizeof(uint);
        }
        int Counter = BitConverter.ToInt32(Bytes, Offset);
        return new TokenStoreInventory(Merchandises, Counter);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not TokenStoreInventory Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(TokenStoreInventory)}'.", nameof(Value));

        for (uint i = 0; i < RewardsManager.MAX_INVENTORY; i++)
        {
            BitConverter.GetBytes(Value2.InventoryList[i]?.Address ?? 0).CopyTo(Buffer, Offset);
            Offset += sizeof(uint);
        }
        BitConverter.GetBytes(Value2.Counter).CopyTo(Buffer, Offset);
    }
}
