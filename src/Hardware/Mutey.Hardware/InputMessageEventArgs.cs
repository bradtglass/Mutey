namespace Mutey.Hardware
{
    using System;
    using Mutey.Core.Input;

    /// <summary>
    ///     Event args for user input of a mute button.
    /// </summary>
    public class InputMessageEventArgs : EventArgs
    {
        /// <summary>
        ///     The requested message type.
        /// </summary>
        public InputMessageKind Message { get; }

        /// <summary>
        ///     The unique ID of the device raising the message.
        /// </summary>
        public string DeviceId { get; }

        public InputMessageEventArgs( InputMessageKind message, string deviceId )
        {
            Message = message;
            DeviceId = deviceId;
        }
    }
}
