using System;
using System.Collections.Generic;

namespace Mutey.Input
{
    public interface IMuteHardwareDetector
    {
        /// <summary>
        ///     Attempts to detect possible mute hardware devices.
        /// </summary>
        IEnumerable<PossibleMuteHardware> Find();

        /// <summary>
        ///     Occurs when the hardware changes.
        /// </summary>
        event EventHandler? HardwareChanged;
    }
}