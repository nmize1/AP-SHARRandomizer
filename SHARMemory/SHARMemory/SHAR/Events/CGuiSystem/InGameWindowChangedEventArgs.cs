using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.CGuiSystem;
public class InGameWindowChangedEventArgs : EventArgs
{
    public CGuiManager.WindowID OldID { get; }
    public CGuiManager.WindowID NewID { get; }
    public CGuiWindow NewWindow { get; }

    public InGameWindowChangedEventArgs(CGuiManager.WindowID oldID, CGuiManager.WindowID newID, CGuiWindow newWindow)
    {
        OldID = oldID;
        NewID = newID;
        NewWindow = newWindow;
    }
}