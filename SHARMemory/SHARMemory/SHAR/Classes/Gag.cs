using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using SHARMemory.SHAR.Structs.InteriorManager;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVGag@@")]
public class Gag : ButtonHandler
{
    public Gag(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint AnimationPlayerLoadDataCallBackVFTableOffset = ActionButtonOffset + sizeof(uint);

    internal const uint EventListenerVFTableOffset = AnimationPlayerLoadDataCallBackVFTableOffset + sizeof(uint);

    internal const uint NISSoundPlaybackCompleteCallbackVFTableOffset = EventListenerVFTableOffset + sizeof(uint);

    internal const uint DialogEventDataOffset = NISSoundPlaybackCompleteCallbackVFTableOffset + sizeof(uint);
    public DialogEventData DialogEventData
    {
        get => ReadStruct<DialogEventData>(DialogEventDataOffset);
        set => WriteStruct(DialogEventDataOffset, value);
    }

    internal const uint BindingOffset = DialogEventDataOffset + DialogEventData.Size;
    public GagBinding? Binding
    {
        get
        {
            var address = ReadUInt32(BindingOffset);
            if (address == 0)
                return null;
            return Memory.ReadStruct<GagBinding>(address);
        }
        set
        {
            var address = ReadUInt32(BindingOffset);
            if (address == 0)
                throw new System.NotImplementedException("Cannot add a new GagBinding");
            Memory.WriteStruct(address, value);
        }
    }

    internal const uint LocatorOffset = BindingOffset + sizeof(uint);
    public EventLocator Locator => Memory.ClassFactory.Create<EventLocator>(ReadUInt32(LocatorOffset));

    internal const uint TriggerOffset = LocatorOffset + sizeof(uint);
    //public SphereTriggerVolume Trigger => Memory.ClassFactory.Create<SphereTriggerVolume>(ReadUInt32(TriggerOffset));

    internal const uint DrawOffset = TriggerOffset + sizeof(uint);
    //public GagDrawable Draw => Memory.ClassFactory.Create<GagDrawable>(ReadUInt32(DrawOffset));

    internal const uint GagPlayerOffset = DrawOffset + sizeof(uint);
    //public NISPlayer GagPlayer => Memory.ClassFactory.Create<NISPlayer>(ReadUInt32(GagPlayerOffset));

    internal const uint CollObjOffset = GagPlayerOffset + sizeof(uint);
    public CollisionObject CollObj => Memory.ClassFactory.Create<CollisionObject>(ReadUInt32(CollObjOffset));

    internal const uint LoadedOffset = CollObjOffset + sizeof(uint);
    public bool Loaded
    {
        get => ReadBoolean(LoadedOffset);
        set => WriteBoolean(LoadedOffset, value);
    }

    internal const uint LoadingOffset = LoadedOffset + 1;
    public bool Loading
    {
        get => ReadBoolean(LoadingOffset);
        set => WriteBoolean(LoadingOffset, value);
    }

    internal const uint TimeToCameraShakeOffset = LoadingOffset + 3; // Padding
    public int TimeToCameraShake
    {
        get => ReadInt32(TimeToCameraShakeOffset);
        set => WriteInt32(TimeToCameraShakeOffset, value);
    }

    internal const uint TimeToCameraShakeEndOffset = TimeToCameraShakeOffset + sizeof(int);
    public int TimeToCameraShakeEnd
    {
        get => ReadInt32(TimeToCameraShakeEndOffset);
        set => WriteInt32(TimeToCameraShakeEndOffset, value);
    }

    internal const uint TimeToCoinSpawnOffset = TimeToCameraShakeEndOffset + sizeof(int);
    public int TimeToCoinSpawn
    {
        get => ReadInt32(TimeToCoinSpawnOffset);
        set => WriteInt32(TimeToCoinSpawnOffset, value);
    }

    internal const uint SparkleOffset = TimeToCoinSpawnOffset + sizeof(int);
    public bool Sparkle
    {
        get => ReadBoolean(SparkleOffset);
        set => WriteBoolean(SparkleOffset, value);
    }

    internal const uint SoundLoadedOffset = SparkleOffset + 1;
    public bool SoundLoaded
    {
        get => ReadBoolean(SoundLoadedOffset);
        set => WriteBoolean(SoundLoadedOffset, value);
    }

    internal const uint PlayingOffset = SoundLoadedOffset + 1;
    public bool Playing
    {
        get => ReadBoolean(PlayingOffset);
        set => WriteBoolean(PlayingOffset, value);
    }

    internal const uint IsNISSoundCompleteOffset = PlayingOffset + 1;
    public bool IsNISSoundComplete
    {
        get => ReadBoolean(IsNISSoundCompleteOffset);
        set => WriteBoolean(IsNISSoundCompleteOffset, value);
    }
}
