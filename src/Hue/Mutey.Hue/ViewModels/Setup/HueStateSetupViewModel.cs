using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Mutey.Hue.ViewModels.Setup;

public class HueStateSetupViewModel : ObservableObject
{
    public HueStateSetupViewModel(bool isActive, string title)
    {
        IsActive = isActive;
        Title = title;
    }

    internal bool IsActive { get; }

    public string Title { get; }

    public ObservableCollection<LightSetupViewModel> Lights { get; } = new();
}