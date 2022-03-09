namespace Mutey.Hue.Mic
{
    using System;

    public class MicrophoneActivityEventArgs : EventArgs
    {
        public bool IsActive { get; }

        public MicrophoneActivityEventArgs( bool isActive )
        {
            IsActive = isActive;
        }
    }
}
