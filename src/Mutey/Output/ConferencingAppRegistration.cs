using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mutey.Output
{
    public abstract class ConferencingAppRegistration : IConferencingAppRegistration
    {
        protected abstract string ProcessName { get; }

        protected abstract string IconName { get; }

        public abstract string Name { get; }

        public bool IsMatch(Process process)
            => process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase);

        public ImageSource LoadIcon()
        {
            BitmapDecoder decoder = BitmapDecoder.Create(new Uri($"Icons/{IconName}"), BitmapCreateOptions.None,
                BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        public abstract IConferenceConnection Connect(Process process);
    }
}