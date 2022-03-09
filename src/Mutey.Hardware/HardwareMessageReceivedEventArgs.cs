namespace Mutey.Hardware
{
    using System;

    /// <summary>
    ///     Event args for user input of a mute button.
    /// </summary>
    public class HardwareMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        ///     The requested message type.
        /// </summary>
        public HardwareMessageType Message { get; }

        public HardwareType Hardware { get; }

        public HardwareMessageReceivedEventArgs( HardwareMessageType message, HardwareType hardware )
        {
            Message = message;
            Hardware = hardware;
        }
    }
}
