namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Interface for a class that controls mute button hardware.
    /// </summary>
    public interface IMuteDeviceManager
    {
        IMuteDevice? CurrentDevice { get; }

        IEnumerable<PossibleMuteDevice> AvailableDevices { get; }

        void ChangeDevice( PossibleMuteDevice? newDevice );

        event EventHandler<CurrentDeviceChangedEventArgs>? CurrentDeviceChanged;

        event EventHandler<EventArgs>? AvailableDevicesChanged;

        public void RegisterDeviceDetector( IMuteDeviceDetector detector );
    }
}
