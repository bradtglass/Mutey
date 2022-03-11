namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;

    public interface IMuteDeviceDetector
    {
        /// <summary>
        ///     Attempts to detect possible mute hardware devices.
        /// </summary>
        IEnumerable<PossibleMuteDevice> Find();

        /// <summary>
        ///     Occurs when the hardware changes.
        /// </summary>
        event EventHandler? DevicesChanged;
    }
}
