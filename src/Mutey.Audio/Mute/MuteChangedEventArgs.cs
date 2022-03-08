using System;

namespace Mutey.Audio.Mute
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