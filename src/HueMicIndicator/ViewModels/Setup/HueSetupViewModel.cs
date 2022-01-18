using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HueMicIndicator.Hue;
using HueMicIndicator.Hue.State;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HueMicIndicator.ViewModels.Setup;

public class HueSetupViewModel : ObservableObject
{
    private readonly HueContext context;

    private IReadOnlyCollection<SelectableViewModel<LightInfo>>? selectableLights;

    private IReadOnlyList<HueStateSetupViewModel>? states;

    public HueSetupViewModel(HueContext context)
    {
        this.context = context;

        SaveCommand = new RelayCommand(Save);
        LoadLightsCommand = new AsyncRelayCommand(LoadLights);
    }

    public IReadOnlyCollection<SelectableViewModel<LightInfo>>? SelectableLights
    {
        get => selectableLights;
        private set => SetProperty(ref selectableLights, value);
    }

    public ICommand LoadLightsCommand { get; }

    public ICommand SaveCommand { get; }

    public IReadOnlyList<HueStateSetupViewModel>? States
    {
        get => states;
        private set => SetProperty(ref states, value);
    }

    private async Task LoadLights()
    {
        List<HueStateSetupViewModel> viewModels = new()
        {
            new HueStateSetupViewModel(true, "Active"),
            new HueStateSetupViewModel(false, "Inactive")
        };
        
        States ??= viewModels;
        
        IReadOnlyCollection<LightInfo> lights = await Task.Run(context.GetLightsAsync);
        ConfigureFromLights(lights);
    }


    private void ConfigureFromLights(IEnumerable<LightInfo> lights)
    {
        if (SelectableLights != null)
            return;
        
        List<SelectableViewModel<LightInfo>> viewModels =
            lights.Select(l => new SelectableViewModel<LightInfo>(l)).ToList();

        SelectableLights = viewModels;

        InitializeFromCurrentSettings(viewModels);

        foreach (SelectableViewModel<LightInfo> viewModel in viewModels)
            viewModel.PropertyChanged += SelectableChanged;
    }

    private void InitializeFromCurrentSettings(List<SelectableViewModel<LightInfo>> viewModels)
    {
        var activeSetting = context.StateStore.Get(true);
        var inactiveSetting = context.StateStore.Get(false);

        var lights = activeSetting.Lights.Keys.Concat(inactiveSetting.Lights.Keys)
            .Distinct(StringComparer.Ordinal)
            .Select(n => viewModels.FirstOrDefault(vm => string.Equals(vm.Value.Name, n)))
            .Where(vm => vm != null);

        foreach (var light in lights)
        {
            light!.IsSelected = true;
            InitializeFromCurrentSettings(light.Value);
        }
    }

    private void InitializeFromCurrentSettings(LightInfo light)
        => ForEachState(s =>
        {
            var setting = context.StateStore.Get(s.IsActive);
            InitializeFromCurrentSettings(s, setting, light);
        });

    private static void InitializeFromCurrentSettings(HueStateSetupViewModel viewModel, HueStateSetting setting, LightInfo light)
    {
        var lightSetting = setting.Lights.GetValueOrDefault(light.Name);

        if (viewModel.Lights.FirstOrDefault(l => l.Info.Name == light.Name) is { })
            return;

        var lightViewModel = new LightSetupViewModel(light, lightSetting);
        viewModel.Lights.Add(lightViewModel);
    }

    private void SelectableChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectableViewModel<LightInfo>.IsSelected))
            return;

        SelectableViewModel<LightInfo> selectable = (SelectableViewModel<LightInfo>)sender!;

        if (selectable.IsSelected)
            ForEachState(s => EnsureEditable(s, selectable.Value));
        else
            ForEachState(s => EnsureNotEditable(s, selectable.Value));
    }

    private void ForEachState(Action<HueStateSetupViewModel> callback)
    {
        foreach (var viewModel in States ?? Array.Empty<HueStateSetupViewModel>()) callback(viewModel);
    }

    private static void EnsureNotEditable(HueStateSetupViewModel viewModel, LightInfo lightInfo)
    {
        for (var i = viewModel.Lights.Count - 1; i >= 0; i--)
            if (viewModel.Lights[i].Info.Equals(lightInfo))
                viewModel.Lights.RemoveAt(i);
    }

    private void EnsureEditable(HueStateSetupViewModel viewModel, LightInfo lightInfo)
    {
        if (viewModel.Lights.Any(s => s.Info.Equals(lightInfo)))
            return;

        InitializeFromCurrentSettings(lightInfo);
    }

    private void Save()
    {
        if (States == null)
            return;

        foreach (var state in States)
        {
            var setting = GetSetting(state);
            context.StateStore.Set(state.IsActive, setting);
        }
    }

    private static HueStateSetting GetSetting(HueStateSetupViewModel viewModel)
        => new(viewModel.Lights.ToDictionary(s => s.Info.Name, s => s.GetSetting()));
}