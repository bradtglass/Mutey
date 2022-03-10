namespace Mutey.Core.Audio
{
    /// <summary>
    ///     The possible states for audio input.
    /// </summary>
    public enum MuteState
    {
        /// <summary>
        ///     Audio input state is not known/detectable.
        /// </summary>
        Unknown,

        /// <summary>
        ///     Audio input is disabled.
        /// </summary>
        Muted,

        /// <summary>
        ///     Audio input is enabled.
        /// </summary>
        Unmuted
    }
}
