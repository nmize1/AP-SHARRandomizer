using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

public class CharacterRenderable : Class
{
    public CharacterRenderable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public PointerArray<tDrawablePose> DrawableList => new(Memory, Address, 3);

    public int CurrentLOD
    {
        get => ReadInt32(12);
        set => WriteInt32(12, value);
    }

    public bool InAnyonesFrustrum
    {
        get => ReadBoolean(16);
        set => WriteBoolean(16, value);
    }

    public Texture SwatchTexture => Memory.ClassFactory.Create<Texture>(ReadUInt32(20));

    public PointerArray<Texture> SwatchTextures => new(Memory, Address + 24, 5);

    public tShader SwatchShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(44));
}
