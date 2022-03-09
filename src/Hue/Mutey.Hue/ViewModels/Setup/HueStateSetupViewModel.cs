namespace Mutey.Hue.ViewModels.Setup
{
    using System.Collections.ObjectModel;
    using Microsoft.Toolkit.Mvvm.ComponentModel;

    public class HueStateSetupViewModel : ObservableObject
    {
        internal bool IsActive { get; }

        public string Title { get; }

        public ObservableCollection<LightSetupViewModel> Lights { get; } = new();

        public HueStateSetupViewModel( bool isActive, string title )
        {
            IsActive = isActive;
            Title = title;
        }
    }
}
