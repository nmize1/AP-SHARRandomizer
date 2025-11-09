using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiSystem@@")]
public class CGuiSystem : CGuiEntity
{
    public enum States
    {
        GUIUninitialized,
        GUIIdle,

        LanguageLoading,
        LanguageActive,

        BootupLoading,
        BootupActive,

        BackendLoading,
        FrontendLoadingDuringBootup,

        FrontendLoading,
        FrontendActive,

        MinigameLoading,
        MinigameActive,

        IngameLoading,
        IngameActive,

        DemoActive
    }

    public CGuiSystem(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyLoadProjectCallbackVFTableOffset = ParentOffset + sizeof(uint);

    internal const uint GameDataHandlerVFTableOffset = ScroobyLoadProjectCallbackVFTableOffset + sizeof(uint);

    internal const uint StateOffset = GameDataHandlerVFTableOffset + sizeof(uint);
    public States State
    {
        get => (States)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint TextBibleOffset = StateOffset + sizeof(uint);
    public CGuiTextBible TextBible => Memory.ClassFactory.Create<CGuiTextBible>(ReadUInt32(TextBibleOffset));

    internal const uint ManagerLanguageOffset = TextBibleOffset + sizeof(uint);
    public CGuiManagerLanguage ManagerLanguage => Memory.ClassFactory.Create<CGuiManagerLanguage>(ReadUInt32(ManagerLanguageOffset));

    internal const uint ManagerBootUpOffset = ManagerLanguageOffset + sizeof(uint);
    public CGuiManagerBootUp ManagerBootUp => Memory.ClassFactory.Create<CGuiManagerBootUp>(ReadUInt32(ManagerBootUpOffset));

    internal const uint ManagerBackEndOffset = ManagerBootUpOffset + sizeof(uint);
    public CGuiManagerBackEnd ManagerManagerBackEnd => Memory.ClassFactory.Create<CGuiManagerBackEnd>(ReadUInt32(ManagerBackEndOffset));

    internal const uint ManagerFrontEndOffset = ManagerBackEndOffset + sizeof(uint);
    public CGuiManagerFrontEnd ManagerFrontEnd => Memory.ClassFactory.Create<CGuiManagerFrontEnd>(ReadUInt32(ManagerFrontEndOffset));

    internal const uint ManagerMiniGameOffset = ManagerFrontEndOffset + sizeof(uint);
    public CGuiManagerMiniGame ManagerMiniGame => Memory.ClassFactory.Create<CGuiManagerMiniGame>(ReadUInt32(ManagerMiniGameOffset));

    internal const uint ManagerInGameOffset = ManagerMiniGameOffset + sizeof(uint);
    public CGuiManagerInGame ManagerInGame => Memory.ClassFactory.Create<CGuiManagerInGame>(ReadUInt32(ManagerInGameOffset));
}
