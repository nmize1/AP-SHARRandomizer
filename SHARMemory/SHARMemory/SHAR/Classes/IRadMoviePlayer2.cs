using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AUIRadMoviePlayer2@@")]
public class IRadMoviePlayer2 : IRefCount
{
    public enum States
    {
        NoData,
        Loading,
        LoadToPlay,
        ReadyToPlay,
        Playing,
    }

    public IRadMoviePlayer2(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }
}
