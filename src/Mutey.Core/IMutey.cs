namespace Mutey.Core
{
    using System;
    using Mutey.Core.Audio;
    using Mutey.Core.Input;

    /// <summary>
    ///     Exposes core business logic for managing software mute and handling inputs/outputs.
    /// </summary>
    public interface IMutey
    {
        /// <summary>
        ///     Transforms an input message using the default transformer.
        /// </summary>
        public void TransformInput( string deviceId, InputMessageKind messageKind );

        /// <summary>
        ///     Changes the current state of target microphone(s).
        /// </summary>
        public void ChangeMuteState( MuteState state );

        /// <summary>
        ///     Raised when the state of the target microphone(s) is changed.
        /// </summary>
        public event EventHandler<MuteChangedEventArgs>? StateChanged;
    }
}
