using System.Collections.Generic;
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

    public HueSetupViewModel(HueContext context)
    {
        this.context = context;

        SaveCommand = new RelayCommand(Save);
        LoadLightsCommand = new AsyncRelayCommand(LoadLights);
    }

    private async Task LoadLights()
    {
        IReadOnlyCollection<LightInfo> lights = await Task.Run(context.GetLightsAsync);

        List<HueStateSetupViewModel> viewModels = new()
        {
            new HueStateSetupViewModel(true, "Active", lights),
            new HueStateSetupViewModel(false, "Inactive", lights)
        };

        States ??= viewModels;
    }

    public ICommand LoadLightsCommand { get; }

    public ICommand SaveCommand { get; }


    private IReadOnlyList<HueStateSetupViewModel>? states;

    public IReadOnlyList<HueStateSetupViewModel>? States
    {
        get => states;
        private set => SetProperty(ref states, value);
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

        return new HueLightSetting(viewModel.On, null,color);
    }
}