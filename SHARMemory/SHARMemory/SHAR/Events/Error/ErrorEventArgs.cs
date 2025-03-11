using System;

namespace SHARMemory.SHAR.Events.Error;

public class ErrorEventArgs : EventArgs
{
    public Exception Exception { get; }

    public ErrorEventArgs(Exception exception)
    {
        Exception = exception;
    }
}