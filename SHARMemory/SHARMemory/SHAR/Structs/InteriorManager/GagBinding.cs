using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Text;

namespace SHARMemory.SHAR.Structs.InteriorManager;

[Struct(typeof(GagBindingStruct))]
public struct GagBinding
{
    public const int Size = sizeof(ulong) + sizeof(bool) + 3 /* padding */ + sizeof(int) + 13 + 3 /* padding */ + sizeof(uint) + sizeof(bool) + sizeof(bool) + sizeof(bool) + sizeof(bool) + sizeof(ulong) + Vector3.Size + sizeof(bool) + 3 /* padding */ + sizeof(ulong) + Vector3.Size + sizeof(float) + sizeof(bool) + 13 + 2 /* padding */ + sizeof(uint) + sizeof(bool) + 3 /* padding */ + ShakeEventData.Size + 3 /* padding */ + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(uint) + sizeof(uint) + sizeof(uint) + 30 + 30 + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(bool) + sizeof(bool) + 2 /* padding */ + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(int);

    public ulong InteriorUID;
    public bool Random;
    public int Weight;
    public string GagFileName;
    public tFrameController.P3DCycleMode CycleMode;
    public bool Triggered;
    public bool Action;
    public bool Retrigger;
    public bool UseGagLocator;
    public ulong GagLoc;
    public Vector3 GagPos;
    public bool UseTriggerLocator;
    public ulong TriggerLoc;
    public Vector3 TriggerPos;
    public float TriggerRadius;
    public byte ISMovie;
    public string GagFMVFileName;
    public uint SoundID;
    public bool CameraShake;
    public ShakeEventData Shake;
    public float ShakeDelay;
    public float ShakeDuration;
    public float CoinDelay;
    public uint Coins;
    public uint LoopIntro;
    public uint LoopOutro;
    public string DialogChar1;
    public string DialogChar2;
    public uint AcceptDialogID;
    public uint RejectDialogID;
    public uint InstructDialogID;
    public bool Sparkle;
    public bool AnimBV;
    public uint LoadDist;
    public uint UnloadDist;
    public uint SoundLoadDist;
    public uint SoundUnloadDist;
    public int PersistIndex;

    public GagBinding(ulong interiorUID, bool random, int weight, string gagFileName, tFrameController.P3DCycleMode cycleMode, bool triggered, bool action, bool retrigger, bool useGagLocator, ulong gagLoc, Vector3 gagPos, bool useTriggerLocator, ulong triggerLoc, Vector3 triggerPos, float triggerRadius, byte isMovie, string gagFMVFilename, uint soundID, bool cameraShake, ShakeEventData shake, float shakeDelay, float shakeDuration, float coinDelay, uint coins, uint loopIntro, uint loopOutro, string dialogChar1, string dialogChar2, uint acceptDialogID, uint rejectDialogID, uint instructDialogID, bool sparkle, bool animBV, uint loadDist, uint unloadDist, uint soundLoadDist, uint soundUnloadDist, int persistIndex)
    {
        InteriorUID = interiorUID;
        Random = random;
        Weight = weight;
        GagFileName = gagFileName;
        CycleMode = cycleMode;
        Triggered = triggered;
        Action = action;
        Retrigger = retrigger;
        UseGagLocator = useGagLocator;
        GagLoc = gagLoc;
        GagPos = gagPos;
        UseTriggerLocator = useTriggerLocator;
        TriggerLoc = triggerLoc;
        TriggerPos = triggerPos;
        TriggerRadius = triggerRadius;
        ISMovie = isMovie;
        GagFMVFileName = gagFMVFilename;
        SoundID = soundID;
        CameraShake = cameraShake;
        Shake = shake;
        ShakeDelay = shakeDelay;
        CoinDelay = coinDelay;
        Coins = coins;
        LoopIntro = loopIntro;
        LoopOutro = loopOutro;
        DialogChar1 = dialogChar1;
        DialogChar2 = dialogChar2;
        AcceptDialogID = acceptDialogID;
        RejectDialogID = rejectDialogID;
        InstructDialogID = instructDialogID;
        Sparkle = sparkle;
        AnimBV = animBV;
        LoadDist = loadDist;
        UnloadDist = unloadDist;
        SoundLoadDist = soundLoadDist;
        SoundUnloadDist = soundUnloadDist;
        PersistIndex = persistIndex;
    }

    public override readonly string ToString() => $"TODO";
}

internal class GagBindingStruct : Struct
{
    public override int Size => GagBinding.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        ulong InteriorUID = BitConverter.ToUInt64(Bytes, Offset);
        Offset += sizeof(ulong);
        bool Random = BitConverter.ToBoolean(Bytes, Offset);
        Offset += 4;
        int Weight = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        string GagFileName = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 13));
        Offset += 16;
        tFrameController.P3DCycleMode CycleMode = (tFrameController.P3DCycleMode)BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        bool Triggered = BitConverter.ToBoolean(Bytes, Offset);
        Offset++;
        bool Action = BitConverter.ToBoolean(Bytes, Offset);
        Offset++;
        bool Retrigger = BitConverter.ToBoolean(Bytes, Offset);
        Offset++;
        bool UseGagLocator = BitConverter.ToBoolean(Bytes, Offset);
        Offset++;
        ulong GagLoc = BitConverter.ToUInt64(Bytes, Offset);
        Offset += sizeof(ulong);
        Vector3 GagPos = Memory.StructFromBytes<Vector3>(Bytes, Offset);
        Offset += Vector3.Size;
        bool UseTriggerLocator = BitConverter.ToBoolean(Bytes, Offset);
        Offset += 4;
        ulong TriggerLoc = BitConverter.ToUInt64(Bytes, Offset);
        Offset += sizeof(ulong);
        Vector3 TriggerPos = Memory.StructFromBytes<Vector3>(Bytes, Offset);
        Offset += Vector3.Size;
        float TriggerRadius = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        byte ISMovie = Bytes[Offset];
        Offset += 4;
        string GagFMVFileName = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 13));
        Offset += 16;
        uint SoundID = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        bool CameraShake = BitConverter.ToBoolean(Bytes, Offset);
        Offset += 4;
        ShakeEventData Shake = Memory.StructFromBytes<ShakeEventData>(Bytes, Offset);
        Offset += ShakeEventData.Size;
        float ShakeDelay = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        float ShakeDuration = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        float CoinDelay = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        uint Coins = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint LoopIntro = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint LoopOutro = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        string DialogChar1 = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 30));
        Offset += 30;
        string DialogChar2 = ProcessMemory.NullTerminate(Encoding.UTF8.GetString(Bytes, Offset, 30));
        Offset += 30;
        uint AcceptDialogID = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint RejectDialogID = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint InstructDialogID = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        bool Sparkle = BitConverter.ToBoolean(Bytes, Offset);
        Offset++;
        bool AnimBV = BitConverter.ToBoolean(Bytes, Offset);
        Offset += 3;
        uint LoadDist = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint UnloadDist = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint SoundLoadDist = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        uint SoundUnloadDist = BitConverter.ToUInt32(Bytes, Offset);
        Offset += sizeof(uint);
        int PersistIndex = BitConverter.ToInt32(Bytes, Offset);
        return new GagBinding(InteriorUID, Random, Weight, GagFileName, CycleMode, Triggered, Action, Retrigger, UseGagLocator, GagLoc, GagPos, UseTriggerLocator, TriggerLoc, TriggerPos, TriggerRadius, ISMovie, GagFMVFileName, SoundID, CameraShake, Shake, ShakeDelay, ShakeDuration, CoinDelay, Coins, LoopIntro, LoopOutro, DialogChar1, DialogChar2, AcceptDialogID, RejectDialogID, InstructDialogID, Sparkle, AnimBV, LoadDist, UnloadDist, SoundLoadDist, SoundUnloadDist, PersistIndex);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not GagBinding Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(GagBinding)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.InteriorUID).CopyTo(Buffer, Offset);
        Offset += sizeof(ulong);
        BitConverter.GetBytes(Value2.Random).CopyTo(Buffer, Offset);
        Offset += 4;
        BitConverter.GetBytes(Value2.Weight).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        Memory.GetStringBytes(Value2.GagFileName, Encoding.UTF8, 13).CopyTo(Buffer, Offset);
        Offset += 16;
        BitConverter.GetBytes((int)Value2.CycleMode).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.Triggered).CopyTo(Buffer, Offset);
        Offset++;
        BitConverter.GetBytes(Value2.Action).CopyTo(Buffer, Offset);
        Offset++;
        BitConverter.GetBytes(Value2.Retrigger).CopyTo(Buffer, Offset);
        Offset++;
        BitConverter.GetBytes(Value2.UseGagLocator).CopyTo(Buffer, Offset);
        Offset++;
        BitConverter.GetBytes(Value2.GagLoc).CopyTo(Buffer, Offset);
        Offset += sizeof(ulong);
        Memory.BytesFromStruct(Value2.GagPos, Buffer, Offset);
        Offset += Vector3.Size;
        BitConverter.GetBytes(Value2.UseTriggerLocator).CopyTo(Buffer, Offset);
        Offset += 4;
        BitConverter.GetBytes(Value2.TriggerLoc).CopyTo(Buffer, Offset);
        Offset += sizeof(ulong);
        Memory.BytesFromStruct(Value2.TriggerPos, Buffer, Offset);
        Offset += Vector3.Size;
        BitConverter.GetBytes(Value2.TriggerRadius).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        Buffer[Offset] = Value2.ISMovie;
        Offset += 4;
        Memory.GetStringBytes(Value2.GagFMVFileName, Encoding.UTF8, 13).CopyTo(Buffer, Offset);
        Offset += 16;
        BitConverter.GetBytes(Value2.SoundID).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.CameraShake).CopyTo(Buffer, Offset);
        Offset += 4;
        Memory.BytesFromStruct(Value2.Shake, Buffer, Offset);
        Offset += ShakeEventData.Size;
        BitConverter.GetBytes(Value2.ShakeDelay).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.ShakeDuration).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.CoinDelay).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.Coins).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.LoopIntro).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.LoopOutro).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        Memory.GetStringBytes(Value2.DialogChar1, Encoding.UTF8, 30).CopyTo(Buffer, Offset);
        Offset += 30;
        Memory.GetStringBytes(Value2.DialogChar2, Encoding.UTF8, 30).CopyTo(Buffer, Offset);
        Offset += 30;
        BitConverter.GetBytes(Value2.AcceptDialogID).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.RejectDialogID).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.InstructDialogID).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Sparkle).CopyTo(Buffer, Offset);
        Offset++;
        BitConverter.GetBytes(Value2.AnimBV).CopyTo(Buffer, Offset);
        Offset += 3;
        BitConverter.GetBytes(Value2.LoadDist).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.UnloadDist).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.SoundLoadDist).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.SoundUnloadDist).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.PersistIndex).CopyTo(Buffer, Offset);
    }
}
