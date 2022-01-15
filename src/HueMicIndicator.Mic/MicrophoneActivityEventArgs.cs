using System;

namespace HueMicIndicator.Mic
{
    public class MicrophoneActivityEventArgs : EventArgs
    {
        public MicrophoneActivityEventArgs(bool isActive)
        {
            IsActive = isActive;
        }

        public bool IsActive { get; }
    }
}