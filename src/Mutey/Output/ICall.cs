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
        /// Indicates if this call can detect changes to the mute state.
        /// </summary>
        bool CanRaiseMuteStateChanged { get; }

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
        /// Gets the current mute state.
        /// </summary>
        MuteState GetState();
        
        /// <summary>
        /// Raised when the mute state of the call changes, only raised if <see cref="CanRaiseMuteStateChanged"/> is <see langword="true"/>.
        /// </summary>
        event EventHandler? MuteStateChanged; 
        
        /// <summary>
        /// Raised when the call ends.
        /// </summary>
        event EventHandler? Ended;
    }
}