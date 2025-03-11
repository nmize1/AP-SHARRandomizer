using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.SoundManager;

public class DialogPlayingEventArgs : EventArgs
{
    public SelectableDialog Dialog { get; }

    public DialogPlayingEventArgs(SelectableDialog dialog)
    {
        Dialog = dialog;
    }
}