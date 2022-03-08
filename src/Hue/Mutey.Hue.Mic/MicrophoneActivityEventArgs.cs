using System;

namespace Mutey.Hue.Mic;

public class MicrophoneActivityEventArgs : EventArgs
{
    public MicrophoneActivityEventArgs(bool isActive)
    {
        IsActive = isActive;
    }

    public bool IsActive { get; }
}