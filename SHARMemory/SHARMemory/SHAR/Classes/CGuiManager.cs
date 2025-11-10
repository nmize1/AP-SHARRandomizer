using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiManager@@")]
public class CGuiManager : CGuiEntity
{
    public const int NUM_GUI_WINDOW_IDS = 57;
    public const int MAX_WINDOW_HISTORY = 8;

    [Struct(typeof(SHARMemory.Memory.Structs.Int32Struct))]
    public enum WindowID : int
    {
        Undefined = -1,

        // Generic
        GenericMessage,
        GenericPrompt,
        ErrorPrompt,

        // Bootup and Backend
        BootupLoad,
        License,
        Language,
        Loading,
        LoadingFE,
        Demo,
        MemoryCard,
        MemoryCardCheck,
        AutoLoad,

        // Frontend
        Splash,
        IntroTransition,
        MainMenu,
        LoadGame,
        ScrapBook,
        ScrapBookContents,
        ScrapBookStats,
        CardGallery,
        MissionGallery,
        SkinGallery,
        VehicleVallery,
        Options,
        Controller,
        Sound,
        ViewMovies,
        ViewCredits,
        Display,
        PlayMovie,
        PlayMovieDemo,
        PlayMovieIntro,
        PlayMovieNewGame,

        // Ingame
        HUD,
        MultiHUD,
        PauseSunday,
        PauseMission,
        MissionSelect,
        Settings,
        LevelStats,
        ViewCards,
        SaveGame,
        MissionLoad,
        MissionOver,
        MissionSuccess,
        LetterBox,
        PhoneBooth,
        PurchaseRewards,
        IrisWipe,
        LevelEnd,
        Tutorial,

        // Minigame
        MiniMenu,
        MiniHUD,
        MiniPause,
        MiniSummary,
    }

    public enum FrontendState
    {
        Uninitialized,
        ScreenRunning,
        ChangingScreens,
        DynamicLoading,
        ShuttingDown,
        Terminated
    }

    public CGuiManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyGotoScreenCallbackVFTableOffset = ParentOffset + sizeof(uint);
    
    internal const uint ScroobyGotoScreenCallbackScreenOffset = ScroobyGotoScreenCallbackVFTableOffset + sizeof(uint);

    internal const uint EventListenerVFTableOffset = ScroobyGotoScreenCallbackScreenOffset + sizeof(uint);

    internal const uint WindowsOffset = EventListenerVFTableOffset + sizeof(uint);
    public PointerArray<CGuiWindow> Windows => new(Memory, Address + WindowsOffset, NUM_GUI_WINDOW_IDS);

    internal const uint NumWindowsOffset2 = WindowsOffset + sizeof(uint) * NUM_GUI_WINDOW_IDS;
    internal const uint NumWindowsOffset = 248;
    public int NumWindows
    {
        get => ReadInt32(NumWindowsOffset);
        set => WriteInt32(NumWindowsOffset, value);
    }

    internal const uint ScroobyProjectOffset = NumWindowsOffset + sizeof(int);
    public FeEntity ScroobyProject => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(ScroobyProjectOffset));

    internal const uint CurrentScreenOffset = ScroobyProjectOffset + sizeof(uint);
    public WindowID CurrentScreen
    {
        get => (WindowID)ReadUInt32(CurrentScreenOffset);
        set => WriteUInt32(CurrentScreenOffset, (uint)value);
    }

    internal const uint NextScreenOffset = CurrentScreenOffset + sizeof(int);
    public WindowID NextScreen
    {
        get => (WindowID)ReadUInt32(NextScreenOffset);
        set => WriteUInt32(CurrentScreenOffset, (uint)value);
    }

    internal const uint StateOffset = NextScreenOffset + sizeof(int);
    public FrontendState State
    {
        get => (FrontendState)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint WindowHistoryOffset = StateOffset + sizeof(uint);
    public StructArray<WindowID> WindowHistory => new(Memory, ReadUInt32(WindowHistoryOffset), sizeof(int), MAX_WINDOW_HISTORY);

    internal const uint WindowHistoryCountOffset = WindowHistoryOffset + sizeof(int) * MAX_WINDOW_HISTORY;
    public int WindowHistoryCount
    {
        get => ReadInt32(WindowHistoryCountOffset);
        set => WriteInt32(WindowHistoryCountOffset, value);
    }

    internal const uint GotoScreenUserParam1Offset = WindowHistoryCountOffset + sizeof(int);
    public uint GotoScreenUserParam1
    {
        get => ReadUInt32(GotoScreenUserParam1Offset);
        set => WriteUInt32(GotoScreenUserParam1Offset, value);
    }

    internal const uint GotoScreenUserParam2Offset = GotoScreenUserParam1Offset + sizeof(uint);
    public uint GotoScreenUserParam2
    {
        get => ReadUInt32(GotoScreenUserParam2Offset);
        set => WriteUInt32(GotoScreenUserParam2Offset, value);
    }
}
