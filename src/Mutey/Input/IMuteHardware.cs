using System;

namespace Mutey.Input
{
    /// <summary>
    ///     Interface for the handler of a hardware mute button.
    /// </summary>
    public interface IMuteHardware
    {
        /// <summary>
        ///     Occurs when the hardware mute button or switch registers an input.
        /// </summary>
        public event EventHandler<HardwareMessageReceivedEventArgs>? MessageReceived;
    }
}