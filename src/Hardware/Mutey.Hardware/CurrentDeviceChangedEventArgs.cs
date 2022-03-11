namespace Mutey.Hardware
{
    using System;

    public class CurrentDeviceChangedEventArgs : EventArgs
    {
        public IMuteDevice? OldDevice { get; }
        
        public IMuteDevice? NewDevice { get; }

        public CurrentDeviceChangedEventArgs( IMuteDevice? oldDevice, IMuteDevice? newDevice )
        {
            OldDevice = oldDevice;
            NewDevice = newDevice;
        }
    }
}
