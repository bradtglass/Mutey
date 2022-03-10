namespace Mutey.Core.Audio
{
    using System;

    public interface ISystemMuteControl
    {
        public void Mute();

        public void Unmute();

        public MuteState GetState();

        public event EventHandler<MuteChangedEventArgs>? StateChanged;
    }
}
