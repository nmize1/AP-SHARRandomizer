namespace SHARMemory.Memory.RTTI;

//[ClassFactory.TypeInfoName(".?AVtype_info@@")]
public class TypeInfo : Class
{
    //public readonly uint VFTable;
    public readonly uint Spare;
    public readonly string ClassName;

    public TypeInfo(ProcessMemory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        //VFTable = ReadUInt32(0);
        memory.CheckValidMemoryAddress(address + 4);
        Spare = ReadUInt32(4);
        ClassName = ReadNullString(8, System.Text.Encoding.ASCII);

        if (memory.ClassFactory.TypeInfoNames.TryGetValue(ClassName, out string mappedName))
            ClassName = mappedName;
    }

    public override string ToString() => ClassName;
}