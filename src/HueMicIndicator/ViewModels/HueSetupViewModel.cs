using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HueMicIndicator.Hue;
using HueMicIndicator.Hue.State;
using HueMicIndicator.Hue.State.Color;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Q42.HueApi.ColorConverters;

namespace HueMicIndicator.ViewModels;

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
        IReadOnlyCollection<LightInfo> lights = await Task.Run(context.GetLightsAsync);

        ConfigureFromLights(lights);

        List<HueStateSetupViewModel> viewModels = new()
        {
            new HueStateSetupViewModel(true, "Active"),
            new HueStateSetupViewModel(false, "Inactive")
        };

        States ??= viewModels;
    }


    private void ConfigureFromLights(IEnumerable<LightInfo> lights)
    {
        List<SelectableViewModel<LightInfo>> viewModels =
            lights.Select(l => new SelectableViewModel<LightInfo>(l)).ToList();

        foreach (SelectableViewModel<LightInfo> viewModel in viewModels)
            viewModel.PropertyChanged += SelectableChanged;

        SelectableLights = viewModels;
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
        for (var i = viewModel.Setups.Count - 1; i >= 0; i--)
            if (viewModel.Setups[i].Info.Equals(lightInfo))
                viewModel.Setups.RemoveAt(i);
    }

    private static void EnsureEditable(HueStateSetupViewModel viewModel, LightInfo lightInfo)
    {
        if (viewModel.Setups.Any(s => s.Info.Equals(lightInfo)))
            return;

        viewModel.Setups.Add(new LightSetupViewModel(lightInfo));
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
        => new(viewModel.Setups.ToDictionary(s => s.Info.Name, GetSetting));

    private static HueLightSetting GetSetting(LightSetupViewModel viewModel)
    {
        RgbHueColor? color = null;

        if (viewModel.Color is { } vmColor)
            color = new RgbHueColor(new RGBColor(vmColor.R, vmColor.G, vmColor.B));

        return new HueLightSetting(viewModel.On, null, color);
    }
}