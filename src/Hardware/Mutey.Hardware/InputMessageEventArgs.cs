namespace Mutey.Hardware
{
    using System;

    /// <summary>
    ///     Event args for user input of a mute button.
    /// </summary>
    public class InputMessageEventArgs : EventArgs
    {
        /// <summary>
        ///     The requested message type.
        /// </summary>
        public InputMessageKind Message { get; }

        public DeviceKind Device { get; }

        public InputMessageEventArgs( InputMessageKind message, DeviceKind device )
        {
            Message = message;
            Device = device;
        }
    }
}
