using System;

namespace Mutey.Output
{
    /// <summary>
    /// A call that accepts mute control.
    /// </summary>
    public interface ICall
    {
        /// <summary>
        /// The identifier for the call.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Indicates if mute control can be toggled.
        /// </summary>
        bool CanToggle { get; }
     
        /// <summary>
        /// Indicates if mute control can be set explicitly.
        /// </summary>
        bool CanMuteUnmute { get; }

        /// <summary>
        /// Toggle the mute state.
        /// </summary>
        void Toggle();

        /// <summary>
        /// Set mute state to muted.
        /// </summary>
        void Mute();

        /// <summary>
        /// Set the mute state to unmuted.
        /// </summary>
        void Unmute();

        /// <summary>
        /// Raised when the call ends.
        /// </summary>
        event EventHandler? Ended;
    }
}