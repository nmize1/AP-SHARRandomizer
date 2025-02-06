using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCoinManager@@")]
public class CoinManager : Class
{
    public const int NumCoins = 200;
    public enum CoinState
    {
        Inactive,
        InitialSpawning,     // Coins aren't collectable during this time.
        SpawnToCollect,      // Coins spawn and then are collected on first bounce.
        Spawning,            // Still moving, but can be collected.
        Resting,             // Sitting happily.
        RestingIndefinitely, // Sitting happily in the world. I doesn't decay.
        Decaying,            // Spinning away into nothing.
        Collecting,          // Reverse spawning. Attracted to the player.
        Collected,           // Special state held for one frame before the coin heads up into the HUD.
        FlyingToHUD,         // Coin is doing the little fly up animation.
        FlyingFromHUD,       // Coin is doing the little fly down animation.
    }

    public CoinManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    private const uint CoinDrawableOffset = 0x08;
    public tDrawable CoinDrawable => Memory.ClassFactory.Create<tDrawable>(CoinDrawableOffset);

    private const uint CoinBoundingOffset = CoinDrawableOffset + 0x04;
    public Sphere CoinBounding
    {
        get => ReadStruct<Sphere>(Address + CoinBoundingOffset);
        set => WriteStruct(Address + CoinBoundingOffset, value);
    }

    private const uint ActiveCoinsOffset = CoinBoundingOffset + Sphere.Size;
    public StructArray<ActiveCoin> ActiveCoins => new(Memory, ReadUInt32(ActiveCoinsOffset), ActiveCoin.Size, NumCoins);

    private const uint NumActiveCoinsOffset = ActiveCoinsOffset + 0x04;
    public short NumActiveCoins
    {
        get => ReadInt16(NumActiveCoinsOffset);
        set => WriteInt16(NumActiveCoinsOffset, value);
    }

    private const uint NextInactiveCoinOffset = NumActiveCoinsOffset + sizeof(short);
    public short NextInactiveCoin
    {
        get => ReadInt16(NextInactiveCoinOffset);
        set => WriteInt16(NextInactiveCoinOffset, value);
    }

    private const uint NumHUDFlyingOffset = NextInactiveCoinOffset + sizeof(short);
    public short NumHUDFlying
    {
        get => ReadInt16(NumHUDFlyingOffset);
        set => WriteInt16(NumHUDFlyingOffset, value);
    }

    private const uint HUDSparkleOffset = NumHUDFlyingOffset + sizeof(short);
    public short HUDSparkle
    {
        get => ReadInt16(HUDSparkleOffset);
        set => WriteInt16(HUDSparkleOffset, value);
    }

    private const uint HUDCoinXOffset = HUDSparkleOffset + sizeof(short);
    public float HUDCoinX
    {
        get => ReadSingle(HUDCoinXOffset);
        set => WriteSingle(HUDCoinXOffset, value);
    }

    private const uint HUDCoinYOffset = HUDCoinXOffset + sizeof(float);
    public float HUDCoinY
    {
        get => ReadSingle(HUDCoinYOffset);
        set => WriteSingle(HUDCoinYOffset, value);
    }

    private const uint HUDCoinAngleOffset = HUDCoinYOffset + sizeof(float);
    public float HUDCoinAngle
    {
        get => ReadSingle(HUDCoinAngleOffset);
        set => WriteSingle(HUDCoinAngleOffset, value);
    }

    private const uint DrawAfterGuiOffset = HUDCoinAngleOffset + sizeof(float);
    public bool DrawAfterGui
    {
        get => ReadBoolean(DrawAfterGuiOffset);
        set => WriteBoolean(DrawAfterGuiOffset, value);
    }
}
