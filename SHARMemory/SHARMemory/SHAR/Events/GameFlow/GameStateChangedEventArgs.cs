using System;

namespace SHARMemory.SHAR.Events.GameFlow;

public class GameStateChangedEventArgs : EventArgs
{
    public Classes.GameFlow.GameState LastState { get; }
    public Classes.GameFlow.GameState NewState { get; }

    public GameStateChangedEventArgs(Classes.GameFlow.GameState lastState, Classes.GameFlow.GameState newState)
    {
        LastState = lastState;
        NewState = newState;
    }
}