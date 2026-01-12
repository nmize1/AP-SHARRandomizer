using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Events.PresentationManager;

public class CurrentEventChangedEventArgs : EventArgs
{
    public PresentationEvent LastEvent { get; }
    public PresentationEvent NewEvent { get; }

    public CurrentEventChangedEventArgs(PresentationEvent lastEvent, PresentationEvent newEvent)
    {
        LastEvent = lastEvent;
        NewEvent = newEvent;
    }
}