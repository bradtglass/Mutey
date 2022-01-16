using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using HueMicIndicator.Hue;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels;

public class HueStateSetupViewModel : ObservableObject
{
    public HueStateSetupViewModel(bool isActive, string title, IEnumerable<LightInfo> lights)
    {
        IsActive = isActive;
        Title = title;
        SelectableLights = LoadLights(lights);
    }

    internal bool IsActive { get; }
    
    public string Title { get; }

    public ObservableCollection<LightSetupViewModel> Setups { get; } = new();

    public IReadOnlyCollection<SelectableViewModel<LightInfo>>? SelectableLights { get; }

    private IReadOnlyCollection<SelectableViewModel<LightInfo>>? LoadLights(IEnumerable<LightInfo> lights)
    {
        List<SelectableViewModel<LightInfo>> viewModels = lights.Select(l => new SelectableViewModel<LightInfo>(l)).ToList();

        foreach (SelectableViewModel<LightInfo> viewModel in viewModels)
            viewModel.PropertyChanged += SelectableChanged;

        return viewModels;
    }

    private void SelectableChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectableViewModel<LightInfo>.IsSelected))
            return;

        SelectableViewModel<LightInfo> selectable = (SelectableViewModel<LightInfo>)sender!;

        if (selectable.IsSelected)
            EnsureEditable(selectable.Value);
        else
            EnsureNotEditable(selectable.Value);
    }

    private void EnsureNotEditable(LightInfo lightInfo)
    {
        for (var i = Setups.Count - 1; i >= 0; i--)
            if (Setups[i].Info.Equals(lightInfo))
                Setups.RemoveAt(i);
    }

    private void EnsureEditable(LightInfo lightInfo)
    {
        if (Setups.Any(s => s.Info.Equals(lightInfo)))
            return;

        Setups.Add(new LightSetupViewModel(lightInfo));
    }
}