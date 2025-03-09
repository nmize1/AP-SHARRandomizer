using SHARMemory.Memory.RTTI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SHARMemory.Memory;

/// <summary>
/// A class to help manage creating <see cref="Class"/>es based on <see cref="TypeInfoName"/>.
/// </summary>
public class ClassFactory
{
    /// <summary>
    /// An attribute assigned to a <see cref="Class"/> to give the Type Info Name. Also commonly known as the Type Descriptor Name.
    /// </summary>
    public class TypeInfoName : Attribute
    {
        /// <summary>
        /// Gets the <c>TypeInfoName</c> of a given <see cref="Type"/> by checking for a <see cref="TypeInfoName"/> attribute.
        /// </summary>
        /// <param name="Type">
        /// The <c>Type</c> to check.
        /// </param>
        /// <returns>
        /// The <c>TypeInfoName</c> if present, else <c>null</c>.
        /// </returns>
        public static string Get(Type Type)
        {
            var TypeInfoNameAttributes = (TypeInfoName[])Type.GetCustomAttributes(typeof(TypeInfoName), false);
            if (TypeInfoNameAttributes.Length < 1)
                return null;
            var TypeInfoNameAttribute = TypeInfoNameAttributes[0];

            return TypeInfoNameAttribute.Name;
        }

        /// <summary>
        /// The TypeInfoName.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The <c>TypeInfoName</c> constructor.
        /// </summary>
        /// <param name="Name">
        /// The <c>TypeInfoName</c>.
        /// </param>
        public TypeInfoName(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// A generic <see cref="Class"/> for unknown polymorhpic classes.
    /// </summary>
    public class UnknownClass : Class
    {
        /// <summary>
        /// The <c>UnknownClass</c> constructor.
        /// </summary>
        /// <param name="memory">
        /// The <see cref="ProcessMemory"/> manager this class is linked to.
        /// </param>
        /// <param name="address">
        /// The base address of this class in memory.
        /// </param>
        /// <param name="completeObjectLocator">
        /// The <see cref="CompleteObjectLocator"/> of this class.
        /// </param>
        public UnknownClass(ProcessMemory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }
    }

    /// <summary>
    /// Iterates a specific <paramref name="Namespace"/> in a given <paramref name="Assembly"/>, and returns all <see cref="TypeInfoName"/>s of items in that <paramref name="Namespace"/>.
    /// To be used with <seealso cref="AddClasses(IEnumerable{KeyValuePair{string, Type}})"/>.
    /// </summary>
    /// <param name="Assembly">
    /// The <see cref="Assembly"/> the <paramref name="Namespace"/> is in.
    /// </param>
    /// <param name="Namespace">
    /// The <c>namespace</c> to search for.
    /// </param>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> where the <c>Key</c> is the <c>TypeInfoName</c>, and the <c>Value</c> is the <c>Type</c> of the class.
    /// </returns>
    public static Dictionary<string, Type> GetClasses(Assembly Assembly, string Namespace)
    {
        Dictionary<string, Type> Result = [];

        foreach (var Type in Assembly.GetTypes())
        {
            if (Namespace != null && (Type.Namespace != Namespace || Type.Namespace.StartsWith(Namespace + ".")))
                continue;

            var TypeInfoName = ClassFactory.TypeInfoName.Get(Type);
            if (TypeInfoName == null)
                continue;

            Result.Add(TypeInfoName, Type);
        }

        return Result;
    }

    /// <summary>
    /// A <see cref="Dictionary{TKey, TValue}"/> where the <c>Key</c> is the <c>TypeInfoName</c>, and the <c>Value</c> is the <c>Type</c> of the class.
    /// </summary>
    public readonly Dictionary<string, Type> Classes = [];

    /// <summary>
    /// The <see cref="ProcessMemory"/> this <see cref="ClassFactory"/> is linked to.
    /// </summary>
    public readonly ProcessMemory Memory;

    /// <summary>
    /// The <c>ClassFactory</c> constructor.
    /// </summary>
    /// <param name="Memory">
    /// The <see cref="ProcessMemory"/> this <see cref="ClassFactory"/> is linked to.
    /// </param>
    public ClassFactory(ProcessMemory Memory)
    {
        this.Memory = Memory;
    }

    /// <summary>
    /// Adds a list of classes to <see cref="Classes"/>.
    /// To be used with <seealso cref="GetClasses(Assembly, string)"/>.
    /// </summary>
    /// <param name="Classes">
    /// An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> where the <c>Key</c> is the <c>TypeInfoName</c>, and the <c>Value</c> is the <c>Type</c> of the class.
    /// </param>
    public void AddClasses(IEnumerable<KeyValuePair<string, Type>> Classes)
    {
        foreach (var Class in Classes)
            this.Classes[Class.Key] = Class.Value;
    }

    /// <summary>
    /// A <see cref="Dictionary{TKey, TValue}"/> where the <c>Key</c> is the <c>TypeInfoName</c> in the process, and the <c>Value</c> is the <c>TypeInfoName</c> to map to.
    /// </summary>
    public readonly Dictionary<string, string> TypeInfoNames = [];

    /// <summary>
    /// Adds a list of classes to <see cref="Classes"/>.
    /// To be used with <seealso cref="GetClasses(Assembly, string)"/>.
    /// </summary>
    /// <param name="TypeInfoNames">
    /// An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> where the <c>Key</c> is the <c>TypeInfoName</c> in the process, and the <c>Value</c> is the <c>TypeInfoName</c> to map to.
    /// </param>
    public void AddTypeInfoNames(IEnumerable<KeyValuePair<string, string>> TypeInfoNames)
    {
        foreach (var TypeInfoName in TypeInfoNames)
            this.TypeInfoNames[TypeInfoName.Key] = TypeInfoName.Value;
    }

    private Class Create(Type Type, uint Address, CompleteObjectLocator CompleteObjectLocator) => (Class)Activator.CreateInstance(Type, Memory, Address, CompleteObjectLocator);

    private Class CreateNonpolymorphicInternal(Type Type, uint Address) => Create(Type, Address, null);

    /// <summary>
    /// Create an instance of a non-polymorphic <see cref="Class"/> at the given address.
    /// </summary>
    /// <param name="Type">
    /// The <see cref="Class"/> type to use.
    /// </param>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Throws an exception if the given <paramref name="Type"/> is polymorphic.
    /// </exception>
    public Class CreateNonpolymorphic(Type Type, uint Address)
    {
        if (TypeInfoName.Get(Type) != null)
            throw new InvalidCastException($"The given type \"{typeof(Type)}\" is polymorphic. Use \"CreatePolymorphic\" instead.");

        if (Address == 0)
            return null;

        return CreateNonpolymorphicInternal(Type, Address);
    }

    private int PMDToOffset(uint Address, PMD Where)
    {
        var Result = 0;
        if (Where.PDisp >= 0)
        {
            Result = Where.PDisp;
            Result += Memory.ReadInt32((uint)(Memory.ReadUInt32((uint)(Address + Result)) + Where.VDisp));
        }
        Result += Where.MDisp;
        return Result;
    }

    private readonly Dictionary<uint, CompleteObjectLocator> CompleteObjectLocatorCache = [];
    private Class CreatePolymorphicInternal(uint Address, string TypeInfoName = null)
    {
        var FuncTable = Memory.ReadUInt32(Address);
        if (FuncTable == 0)
            return null;
        var CompleteObjectLocatorAddress = (uint)(FuncTable + 0x4 * -1);
        if (!CompleteObjectLocatorCache.TryGetValue(CompleteObjectLocatorAddress, out CompleteObjectLocator CompleteObjectLocator))
        {
            try
            {
                CompleteObjectLocator = Create<CompleteObjectLocator>(Memory.ReadUInt32(CompleteObjectLocatorAddress));
                CompleteObjectLocatorCache.Add(CompleteObjectLocatorAddress, CompleteObjectLocator);
            }
            catch
            {
                return null;
            }
        }

        if (CompleteObjectLocator.Signature != 0)
            throw new InvalidCastException($"{nameof(CompleteObjectLocator)} at 0x{Address:X} has invalid signature");

        Address -= CompleteObjectLocator.Offset;
        if (CompleteObjectLocator.ConstructorDisplacementOffset != 0)
            throw new NotImplementedException();//TODO: This

        PMD? Where = null;
        if (TypeInfoName != null)
        {
            foreach (var BaseClassDescriptor in CompleteObjectLocator.ClassDescriptor.BaseClassArray)
            {
                if (BaseClassDescriptor.TypeInfo.ClassName == TypeInfoName)
                {
                    Where = BaseClassDescriptor.Where;
                    break;
                }
            }
            if (Where == null)
                throw new InvalidCastException($"Given TypeInfoName \"{TypeInfoName}\" is not valid type.\nValid types: {string.Join("; ", CompleteObjectLocator.ClassDescriptor.BaseClassArray.Select(x => x.TypeInfo.ClassName))}");
        }

        foreach (var BaseClassDescriptor in CompleteObjectLocator.ClassDescriptor.BaseClassArray)
        {
            if (Where != null && BaseClassDescriptor.Where != Where)
                continue;

            if (!Classes.TryGetValue(BaseClassDescriptor.TypeInfo.ClassName, out var Type))
                continue;

            return Create(Type, (uint)(Address + PMDToOffset(Address, BaseClassDescriptor.Where)), CompleteObjectLocator);
        }

        if (TypeInfoName != null)
            throw new InvalidCastException($"Given TypeInfoName \"{TypeInfoName}\" is not valid type.\nValid types: {string.Join("; ", CompleteObjectLocator.ClassDescriptor.BaseClassArray.Select(x => x.TypeInfo.ClassName))}");

        return new UnknownClass(Memory, Address, CompleteObjectLocator);
    }

    /// <summary>
    /// Create an instance of a polymorphic <see cref="Class"/> at the given address.
    /// </summary>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <param name="TypeInfoName">
    /// The <c>TypeInfoName</c> of the class if known, else <c>null</c>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    public Class CreatePolymorphic(uint Address, string TypeInfoName = null)
    {
        if (Address == 0)
            return null;

        return CreatePolymorphicInternal(Address, TypeInfoName);
    }

    /// <summary>
    /// Create an instance of a <see cref="Class"/> at the given address.
    /// Will automatically detect if Polymorphic or Non-Polymorphic.
    /// </summary>
    /// <param name="Type">
    /// The <see cref="Class"/> type to use.
    /// </param>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    public Class Create(Type Type, uint Address)
    {
        if (Address == 0)
            return null;

        if (Type.IsAbstract)
            return CreatePolymorphicInternal(Address);

        var TypeInfoName = ClassFactory.TypeInfoName.Get(Type);
        if (TypeInfoName != null)
            return CreatePolymorphicInternal(Address, TypeInfoName);
        else
            return CreateNonpolymorphicInternal(Type, Address);
    }

    /// <summary>
    /// Create an instance of a non-polymorphic <see cref="Class"/> at the given address.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Class"/> type to use.
    /// </typeparam>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    public T CreateNonpolymorphic<T>(uint Address) where T : Class => (T)CreateNonpolymorphic(typeof(T), Address);

    /// <summary>
    /// Create an instance of a polymorphic <see cref="Class"/> at the given address.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Class"/> type to use.
    /// </typeparam>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Throws an exception if the given <typeparamref name="T"/> is non-polymorphic.
    /// </exception>
    public T CreatePolymorphic<T>(uint Address) where T : Class => (T)CreatePolymorphic(Address, TypeInfoName.Get(typeof(T)) ?? throw new InvalidCastException($"The given type \"{typeof(T)}\" is non-polymorphic. Use \"CreateNonpolymorphic\" instead."));

    /// <summary>
    /// Create an instance of a <see cref="Class"/> at the given address.
    /// Will automatically detect if Polymorphic or Non-Polymorphic.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Class"/> type to use.
    /// </typeparam>
    /// <param name="Address">
    /// The base address of the class.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Class"/> or <c>null</c>.
    /// </returns>
    public T Create<T>(uint Address) where T : Class => (T)Create(typeof(T), Address);
}