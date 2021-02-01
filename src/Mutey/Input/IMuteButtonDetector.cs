using System;
using System.Threading.Tasks;

namespace Mutey.Input
{
    /// <summary>
    /// Interface for a class that detects mute button hardware automatically.
    /// </summary>
    public interface IMuteButtonDetector
    {
        /// <summary>
        /// Attempts to detect the connect mute button.
        /// </summary>
        Task<IMuteHardware?> FindButtonAsync();

        /// <summary>
        /// Occurs when a new mute button is detected.
        /// </summary>
        event EventHandler<MuteButtonDetectedEventArgs>? Detected;
    }

    public class MuteButtonDetectedEventArgs : EventArgs
    {
        public Func<IMuteHardware> 
    }
}