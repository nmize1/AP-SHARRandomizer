﻿using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System.Drawing;
using static SHARMemory.SHAR.Globals;

namespace SHARMemory.SHAR.Classes
{
    [ClassFactory.TypeInfoName(".?AVd3dSimpleShader@@")]
#pragma warning disable IDE1006 // Naming Styles
    public class d3dSimpleShader : d3dShader
#pragma warning restore IDE1006 // Naming Styles
    {
        public d3dSimpleShader(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

        public d3dTexture Texture => Memory.ClassFactory.Create<d3dTexture>(ReadUInt32(92));

        public bool UseMultiCBV
        {
            get => ReadBoolean(96);
            set => WriteBoolean(96, value);
        }

        public Color MultiCBVBlendValue
        {
            get => ReadStruct<Color>(100);
            set => WriteStruct(100, value);
        }

        public Color MultiCBVBlendColour
        {
            get => ReadStruct<Color>(104);
            set => WriteStruct(104, value);
        }

        public pddiEnums.pddiMultiCBVBlendMode MultiCBVBlendMode
        {
            get => (pddiEnums.pddiMultiCBVBlendMode)ReadInt32(108);
            set => WriteInt32(108, (int)value);
        }

        public int MultiCBVBlendSetA
        {
            get => ReadInt32(112);
            set => WriteInt32(112, value);
        }

        public int MultiCBVBlendSetB
        {
            get => ReadInt32(116);
            set => WriteInt32(116, value);
        }
    }
}
