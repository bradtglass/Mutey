namespace Mutey.Input
{
    /// <summary>
    ///     The type of change to apply, not all values are supported in all contexts.
    /// </summary>
    public enum HardwareMessageType
    {
        Unknown,
        
        /// <summary>
        ///     Begin toggling the mute state.
        /// </summary>
        StartToggle,

        /// <summary>
        ///     Finish toggling the mute state.
        /// </summary>
        EndToggle,

        /// <summary>
        ///     Mute the audio input.
        /// </summary>
        Mute,

        /// <summary>
        ///     Unmute the audio input.
        /// </summary>
        Unmute
    }
}