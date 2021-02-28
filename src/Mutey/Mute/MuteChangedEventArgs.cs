using System;

namespace Mutey.Mute
{
    public class MuteChangedEventArgs : EventArgs
    {
        public MuteChangedEventArgs(MuteState newState)
        {
            NewState = newState;
        }

        public MuteState NewState { get; }
    }
}