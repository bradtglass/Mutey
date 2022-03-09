namespace Mutey.Input
{
    using System;

    public class CurrentDeviceChangedEventArgs : EventArgs
    {
        public IMuteHardware? OldDevice { get; }
        public IMuteHardware? NewDevice { get; }

        public CurrentDeviceChangedEventArgs( IMuteHardware? oldDevice, IMuteHardware? newDevice )
        {
            OldDevice = oldDevice;
            NewDevice = newDevice;
        }
    }
}
