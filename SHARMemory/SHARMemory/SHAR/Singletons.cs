using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;

namespace SHARMemory.SHAR;

/// <summary>
/// Handles all of SHAR's <see cref="Singleton{T}"/>s.
/// </summary>
public sealed class Singletons
{
    private readonly Singleton<CardGallery> CardGallerySingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.CardGallery"/> singleton.
    /// </summary>
    public CardGallery CardGallery => CardGallerySingleton.Get();

    private readonly Singleton<CharacterManager> CharacterManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.CharacterManager"/> singleton.
    /// </summary>
    public CharacterManager CharacterManager => CharacterManagerSingleton.Get();

    private readonly Singleton<CharacterSheetManager> CharacterSheetManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.CharacterSheetManager"/> singleton.
    /// </summary>
    public CharacterSheetManager CharacterSheetManager => CharacterSheetManagerSingleton.Get();

    private readonly Singleton<CoinManager> CoinManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.CoinManager"/> singleton.
    /// </summary>
    public CoinManager CoinManager => CoinManagerSingleton.Get();

    private readonly Singleton<GameDataManager> GameDataManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.GameDataManager"/> singleton.
    /// </summary>
    public GameDataManager GameDataManager => GameDataManagerSingleton.Get();

    private readonly Singleton<GameFlow> GameFlowSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.GameFlow"/> singleton.
    /// </summary>
    public GameFlow GameFlow => GameFlowSingleton.Get();

    private readonly Singleton<CGuiSystem> GuiSystemSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.CGuiSystem"/> singleton.
    /// </summary>
    public CGuiSystem GuiSystem => GuiSystemSingleton.Get();

    private readonly Singleton<HitNRunManager> HitNRunManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.HitNRunManager"/> singleton.
    /// </summary>
    public HitNRunManager HitNRunManager => HitNRunManagerSingleton.Get();

    private readonly Singleton<InputManager> InputManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.InputManager"/> singleton.
    /// </summary>
    public InputManager InputManager => InputManagerSingleton.Get();

    private readonly Singleton<InteriorManager> InteriorManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.InteriorManager"/> singleton.
    /// </summary>
    public InteriorManager InteriorManager => InteriorManagerSingleton.Get();

    private readonly Singleton<IntersectManager> IntersectManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.IntersectManager"/> singleton.
    /// </summary>
    public IntersectManager IntersectManager => IntersectManagerSingleton.Get();

    private readonly Singleton<LoadingManager> LoadingManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.LoadingManager"/> singleton.
    /// </summary>
    public LoadingManager LoadingManager => LoadingManagerSingleton.Get();

    private readonly Singleton<MissionManager> MissionManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.MissionManager"/> singleton.
    /// </summary>
    public MissionManager MissionManager => MissionManagerSingleton.Get();

    private readonly Singleton<PresentationManager> PresentationManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.PresentationManager"/> singleton.
    /// </summary>
    public PresentationManager PresentationManager => PresentationManagerSingleton.Get();

    private readonly Singleton<RenderFlow> RenderFlowSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.RenderFlow"/> singleton.
    /// </summary>
    public RenderFlow RenderFlow => RenderFlowSingleton.Get();

    private readonly Singleton<RenderManager> RenderManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.RenderManager"/> singleton.
    /// </summary>
    public RenderManager RenderManager => RenderManagerSingleton.Get();

    private readonly Singleton<RewardsManager> RewardsManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.RewardsManager"/> singleton.
    /// </summary>
    public RewardsManager RewardsManager => RewardsManagerSingleton.Get();

    private readonly Singleton<SoundManager> SoundManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.SoundManager"/> singleton.
    /// </summary>
    public SoundManager SoundManager => SoundManagerSingleton.Get();

    private readonly Singleton<TrafficManager> TrafficManagerSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.TrafficManager"/> singleton.
    /// </summary>
    public TrafficManager TrafficManager => TrafficManagerSingleton.Get();

    private readonly Singleton<VehicleCentral> VehicleCentralSingleton;
    /// <summary>
    /// A reference to SHAR's <see cref="Classes.VehicleCentral"/> singleton.
    /// </summary>
    public VehicleCentral VehicleCentral => VehicleCentralSingleton.Get();

    internal Singletons(Memory memory)
    {
        CardGallerySingleton = new(memory, memory.SelectAddress(0x6C9064, 0x6C9024, 0x6C9024, 0x6C905C));
        CharacterManagerSingleton = new(memory, memory.SelectAddress(0x6C8470, 0x6C8430, 0x6C8430, 0x6C8468));
        CharacterSheetManagerSingleton = new(memory, memory.SelectAddress(0x6C8984, 0x6C8944, 0x6C8944, 0x6C897C));
        CoinManagerSingleton = new(memory, memory.SelectAddress(0x6C8450, 0x6C8410, 0x6C8410, 0x6C8448));
        GameFlowSingleton = new(memory, memory.SelectAddress(0x6C9014, 0x6C8FD4, 0x6C8FD4, 0x6C900C));
        GameDataManagerSingleton = new(memory, memory.SelectAddress(0x6C842C, 0x6C83EC, 0x6C83EC, 0x6C8424));
        GuiSystemSingleton = new(memory, memory.SelectAddress(0x6C894C, 0x6C890C, 0x6C890C, 0x6C8944));
        HitNRunManagerSingleton = new(memory, memory.SelectAddress(0x6C84E0, 0x6C84A0, 0x6C84A0, 0x6C84D8));
        InputManagerSingleton = new(memory, memory.SelectAddress(0x6C9008, 0x6C8FC8, 0x6C8FC8, 0x6C9000));
        InteriorManagerSingleton = new(memory, memory.SelectAddress(0x6C8FF8, 0x6C8FB8, 0x6C8FB8, 0x6C8FF0));
        IntersectManagerSingleton = new(memory, memory.SelectAddress(0x6C87A4, 0x6C8764, 0x6C8764, 0x6C879C));
        LoadingManagerSingleton = new(memory, memory.SelectAddress(0x6C8FF4, 0x6C8FB4, 0x6C8FB4, 0x6C8FEC));
        MissionManagerSingleton = new(memory, memory.SelectAddress(0x6C8994, 0x6C8954, 0x6C8954, 0x6C898C));
        PresentationManagerSingleton = new(memory, memory.SelectAddress(0x6C8980, 0x6C8940, 0x6C8940, 0x6C8978));
        RenderFlowSingleton = new(memory, memory.SelectAddress(0x6C87D8, 0x6C8798, 0x6C8798, 0x6C87D0));
        RenderManagerSingleton = new(memory, memory.SelectAddress(0x6C87B4, 0x6C8774, 0x6C8774, 0x6C87AC));
        RewardsManagerSingleton = new(memory, memory.SelectAddress(0x6C8988, 0x6C8948, 0x6C8948, 0x6C8980));
        SoundManagerSingleton = new(memory, memory.SelectAddress(0x6C8590, 0x6C8550, 0x6C8550, 0x6C8588));
        TrafficManagerSingleton = new(memory, memory.SelectAddress(0x6C8468, 0x6C8428, 0x6C8428, 0x6C8460));
        VehicleCentralSingleton = new(memory, memory.SelectAddress(0x6C84D8, 0x6C8498, 0x6C8498, 0x6C84D0));
    }
}