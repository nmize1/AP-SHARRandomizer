using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCCGuiWindow@@")]
public class CGuiWindow : CGuiEntity
{
    public enum WindowID
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

    public enum WindowState
    {
        Uninitialized,
        Intro,
        Running,
        Paused,
        Idle,
        Outro,
        Disabled,
    }

    public CGuiWindow(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint StateOffset = ParentOffset + sizeof(uint);
    public WindowState State
    {
        get => (WindowState)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint PrevStateOffset = StateOffset + sizeof(uint);
    public WindowState PrevState
    {
        get => (WindowState)ReadUInt32(PrevStateOffset);
        set => WriteUInt32(PrevStateOffset, (uint)value);
    }

    internal const uint IDOffset = PrevStateOffset + sizeof(uint);
    public WindowID ID
    {
        get => (WindowID)ReadInt32(IDOffset);
        set => WriteInt32(IDOffset, (int)value);
    }

    internal const uint NumTranspitionsPendingOffset = IDOffset + sizeof(int);
    public int NumTranspitionsPending
    {
        get => ReadInt32(NumTranspitionsPendingOffset);
        set => WriteInt32(NumTranspitionsPendingOffset, value);
    }

    internal const uint FirstTimeEnteredOffset = NumTranspitionsPendingOffset + sizeof(int);
    public bool FirstTimeEntered
    {
        get => ReadBoolean(FirstTimeEnteredOffset);
        set => WriteBoolean(FirstTimeEnteredOffset, value);
    }
}
