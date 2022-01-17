using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels;

public class HueStateSetupViewModel : ObservableObject
{
    public HueStateSetupViewModel(bool isActive, string title)
    {
        IsActive = isActive;
        Title = title;
    }

    internal bool IsActive { get; }

    public string Title { get; }

    public ObservableCollection<LightSetupViewModel> Setups { get; } = new();
}