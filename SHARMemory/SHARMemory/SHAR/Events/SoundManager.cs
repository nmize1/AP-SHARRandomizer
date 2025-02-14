using System;

namespace SHARMemory.SHAR.Events.SoundManager;

public class DialogPlaying : EventArgs
{
    public Classes.SelectableDialog Dialog { get; }

    public DialogPlaying(Classes.SelectableDialog dialog)
    {
        Dialog = dialog;
    }
}