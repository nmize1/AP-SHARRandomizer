using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCharacter@@")]
public class Character : DynaPhysDSG
{
    public Character(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public CharacterController Controller => Memory.ClassFactory.Create<CharacterController>(ReadUInt32(256));

    public CharacterRenderable CharacterRenderable => Memory.ClassFactory.Create<CharacterRenderable>(ReadUInt32(260));

    public float Rotation => ReadSingle(272);

    public Vehicle Car => Memory.ClassFactory.Create<Vehicle>(ReadUInt32(348));

    public JumpAction JumpLocomotion => Memory.ClassFactory.Create<JumpAction>(ReadUInt32(680));
}
