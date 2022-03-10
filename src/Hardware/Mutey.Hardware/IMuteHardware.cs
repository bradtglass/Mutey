namespace Mutey.Hardware
{
    using System;

    /// <summary>
    ///     Interface for the handler of a hardware mute button.
    /// </summary>
    public interface IMuteHardware
    {
        /// <summary>
        ///     The item that generated this hardware.
        /// </summary>
        PossibleMuteHardware Source { get; }

        /// <summary>
        ///     Occurs when the hardware mute button or switch registers an input.
        /// </summary>
        event EventHandler<HardwareMessageReceivedEventArgs>? MessageReceived;
    }
}
