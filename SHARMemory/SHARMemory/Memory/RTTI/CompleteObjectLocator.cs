namespace SHARMemory.Memory.RTTI;

public class CompleteObjectLocator : Class
{
    public readonly uint Signature;
    public readonly uint Offset;
    public readonly uint ConstructorDisplacementOffset;
    public readonly TypeInfo TypeInfo;
    public readonly ClassHierarchyDescriptor ClassDescriptor;

    public CompleteObjectLocator(ProcessMemory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        memory.CheckValidMemoryAddress(address);
        Signature = ReadUInt32(0);
        Offset = ReadUInt32(4);
        ConstructorDisplacementOffset = ReadUInt32(8);
        TypeInfo = Memory.ClassFactory.Create<TypeInfo>(ReadUInt32(12));
        ClassDescriptor = Memory.ClassFactory.Create<ClassHierarchyDescriptor>(ReadUInt32(16));
    }

    public override string ToString() => TypeInfo.ToString();
}