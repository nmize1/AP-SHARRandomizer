using System;

namespace SHARMemory.SHAR.Events.CharacterSheet;

public class PersistentObjectDestroyedEventArts : EventArgs
{
    public Classes.CharacterSheet.PersistentObjectStateSector Sector { get; }
    public uint Index { get; }

    public PersistentObjectDestroyedEventArts(uint index)
    {
        Sector = (Classes.CharacterSheet.PersistentObjectStateSector)(index / 128);
        Index = index % 128;
    }
}