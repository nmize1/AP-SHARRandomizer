using SHARMemory.Memory;
using SHARMemory.SHAR.Structs;
using System;

namespace SHARMemory.SHAR;

public partial class Globals
{
    public sealed class DialogLineHandler
    {
        private readonly Memory Memory;

        internal DialogLineHandler(Memory memory)
        {
            Memory = memory;
        }

        public uint NumberOfCharacters
        {
            get => Memory.ReadUInt32(Memory.SelectAddress(0x64EA90, 0x64EA80, 0x64EA80, 0x64EA90));
            set => Memory.WriteUInt32(Memory.SelectAddress(0x64EA90, 0x64EA80, 0x64EA80, 0x64EA90), value);
        }

        public StructArray<tCharacterCode> Characters => new(Memory, Memory.ReadUInt32(Memory.SelectAddress(0x4D214E + 3, 0x4D220E + 3, 0x4D254E + 3, 0x4D223E + 3)), tCharacterCode.Size, (int)NumberOfCharacters);
    }
}
