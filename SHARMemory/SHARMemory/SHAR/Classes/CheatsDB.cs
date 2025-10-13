using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCheatsDB@@")]
public class CheatsDB : Class
{
    public const int MAX_NUM_POSSIBLE_CHEATS = 256;

    [Struct(typeof(SHARMemory.Memory.Structs.Int32Struct))]
    public enum Cheat : int
    {
        Unregistered = -1,

        MotherOfAllCheats = 0,
        UnlockAllCards = 1,
        UnlockAllOutfits = 2,
        UnlockAllStoryMissions = 3,
        UnlockAllMovies = 4,
        UnlockAllRewardVehicles = 5,
        UnlockEverything = 6,
        NoTopSpeed = 7,
        HighAcceleration = 8,
        CarJumpOnHorn = 9,
        FlamingCar = 10,
        OneTapTrafficDeath = 11,
        x5StageTime = 12,
        ShowAvatarPosition = 13,
        KickSwapsCharacterModel = 14,
        ExtraCoins = 15,
        UnlockAllCameras = 16,
        DemoTest = 17,
        PlayCreditsDialogue = 18,
        ShowSpeedometer = 19,
        RedBrick = 20,
        InvincibleCar = 21,
        ShowTree = 22,
        Trippy = 23
    }

    public CheatsDB(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CheatsDBVFTableOffset = 0;

    internal const uint CheatsOffset = CheatsDBVFTableOffset + sizeof(uint);
    public StructArray<Cheat> Quotes => new(Memory, ReadUInt32(CheatsOffset), sizeof(int), MAX_NUM_POSSIBLE_CHEATS);
}
