using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutey.Output
{
    /// <summary>
    /// A connection to an active conferencing app.
    /// </summary>
    public interface IConferenceConnection : IDisposable
    {
        /// <summary>
        /// Determines if this app will raise the <see cref="CallStarted"/> event, if this is <see langword="false"/> then use <see cref="DefaultCall"/> to access mute control for the entire app.
        /// </summary>
        // TODO This should work? [MemberNotNullWhen(false, nameof(DefaultCall))]
        bool CanDetectNewCalls { get; }
        
        /// <summary>
        /// The app-wide mute controller.
        /// </summary>
        ICall? DefaultCall { get; }

        /// <summary>
        /// Raised when a new call is started, only raised if <see cref="CanDetectNewCalls"/> is <see langword="true"/>.
        /// </summary>
        event EventHandler<ICall>? CallStarted;

        /// <summary>
        /// Raised when the connection is closed.
        /// </summary>
        event EventHandler? Closed;

        void Close();
    }
}