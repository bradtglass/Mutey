namespace Mutey.Audio.Mute
{
    using System;

    public class MuteChangedEventArgs : EventArgs
    {
        public MuteState NewState { get; }

        public MuteChangedEventArgs( MuteState newState )
        {
            NewState = newState;
        }
    }
}
