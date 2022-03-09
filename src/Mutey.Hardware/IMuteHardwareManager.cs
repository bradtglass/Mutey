namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Interface for a class that controls mute button hardware.
    /// </summary>
    public interface IMuteHardwareManager
    {
        IMuteHardware? CurrentDevice { get; }

        IEnumerable<PossibleMuteHardware> AvailableDevices { get; }

        void ChangeDevice( PossibleMuteHardware? newDevice );

        event EventHandler<CurrentDeviceChangedEventArgs>? CurrentDeviceChanged;

        event EventHandler<EventArgs>? AvailableDevicesChanged;

        public void RegisterHardwareDetector( IMuteHardwareDetector detector );
    }
}
