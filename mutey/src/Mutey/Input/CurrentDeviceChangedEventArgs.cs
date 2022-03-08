using System;

namespace Mutey.Input
{
    public class CurrentDeviceChangedEventArgs : EventArgs
    {
        public CurrentDeviceChangedEventArgs(IMuteHardware? oldDevice, IMuteHardware? newDevice)
        {
            OldDevice = oldDevice;
            NewDevice = newDevice;
        }

        public IMuteHardware? OldDevice { get; }
        public IMuteHardware? NewDevice { get; }
    }
}