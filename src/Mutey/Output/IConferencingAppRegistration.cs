using System.Diagnostics;
using System.Windows.Media;

namespace Mutey.Output
{
    /// <summary>
    /// Registration info for a conferencing app.
    /// </summary>
    public interface  IConferencingAppRegistration
    {
        /// <summary>
        /// App name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Determines if the process is an instance of the app.
        /// </summary>
        bool IsMatch(Process process);

        /// <summary>
        /// Loads the apps icon.
        /// </summary>
        ImageSource LoadIcon();

        /// <summary>
        /// Connects to the app.
        /// </summary>
        IConferenceConnection Connect(Process process);
    }
}