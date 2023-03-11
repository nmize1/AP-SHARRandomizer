﻿using SHARMemory.SHAR.Classes;

namespace SHARMemory.SHAR.Pointers
{
    public class CharacterSheetManager : Pointer
    {
        public CharacterSheetManager(Memory memory) : base(memory, memory.SelectAddress(0x6C8984, 0x6C8944, 0x6C8944, 0x6C897C)) { }

        public CharacterSheet CharacterSheet => new(Memory, Value + 4);
    }
}
