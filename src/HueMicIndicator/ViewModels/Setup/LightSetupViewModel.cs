using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using HueMicIndicator.Hue;
using HueMicIndicator.Hue.State;
using HueMicIndicator.Hue.State.Color;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HueMicIndicator.ViewModels.Setup;

public class LightSetupViewModel : ObservableObject
{
    public LightSetupViewModel(LightInfo lightInfo, HueLightSetting? initialSetting)
    {
        Info = lightInfo;
        if (initialSetting != null)
            InitializeFields(initialSetting);

        RefreshAvailable();

        RemoveFieldCommand = new RelayCommand<LightFieldSetupViewModel>(RemoveField);
    }

    public LightInfo Info { get; }

    public ICommand RemoveFieldCommand { get; }

    public ObservableCollection<LightField?> AvailableFields { get; } = new();

    public LightField? NextField
    {
        get => null;
        set
        {
            if (value.HasValue)
                AddField(value.Value);
        }
    }

    public ObservableCollection<LightFieldSetupViewModel> Fields { get; } = new();

    public double MinTemp => 1e6 / (Info.Capabilities.Control.ColorTemperature?.Max ?? 500);

    public double MaxTemp => 1e6 / (Info.Capabilities.Control.ColorTemperature?.Min ?? 153);

    private void RemoveField(LightFieldSetupViewModel? viewModel)
    {
        if (viewModel == null)
            return;

        Fields.Remove(viewModel);
        RefreshAvailable();
    }

    private void AddField(LightField field)
    {
        AddField(GetField(field));
        RefreshAvailable();
    }

    private static LightFieldSetupViewModel GetField(LightField field)
        => field switch
        {
            LightField.On => new LightOnSetupViewModel(),
            LightField.Brightness => new LightBrightnessSetupViewModel(),
            LightField.Color => new LightColorSetupViewModel(),
            LightField.ColorTemperature => new LightColorTempSetupViewModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };

    private void RefreshAvailable()
    {
        AvailableFields.Clear();
        AvailableFields.Add(null);

        if (!Fields.Any(f => f is LightOnSetupViewModel))
            AvailableFields.Add(LightField.On);

        if (!Fields.Any(f => f is LightBrightnessSetupViewModel))
            AvailableFields.Add(LightField.Brightness);

        if (!Fields.Any(f => f is LightColorSetupViewModel or LightColorTempSetupViewModel))
        {
            if (Info.Capabilities.Control.ColorGamut.HasValue)
                AvailableFields.Add(LightField.Color);

            if (Info.Capabilities.Control.ColorTemperature != null)
                AvailableFields.Add(LightField.ColorTemperature);
        }
    }

    private void AddField<T>(Action<T> configure)
        where T : LightFieldSetupViewModel, new()
    {
        T field = new();
        configure(field);

        AddField(field);
    }

    private void AddField(LightFieldSetupViewModel field)
    {
        var type = field.GetType();
        if (Fields.Any(f => f.GetType() == type))
            return;

        foreach (var conflict in Fields.Where(f => f.ConflictsWith(field)).ToList()) Fields.Remove(conflict);

        Fields.Add(field);
    }

    private void InitializeFields(HueLightSetting setting)
    {
        if (setting.On is { } on)
            AddField<LightOnSetupViewModel>(vm => vm.On = on);

        if (setting.Brightness is { } brightness)
            AddField<LightBrightnessSetupViewModel>(vm => vm.SetBrightness(brightness));

        switch (setting.Color)
        {
            case null:
                break;
            case RgbHueColor rgbHueColor:
                var color = Color.FromRgb((byte)rgbHueColor.Color.R,
                    (byte)rgbHueColor.Color.G,
                    (byte)rgbHueColor.Color.B);
                AddField<LightColorSetupViewModel>(vm => vm.Color = color);

                break;
            case TemperatureHueColor temperatureHueColor:
                AddField<LightColorTempSetupViewModel>(vm => vm.Temperature = temperatureHueColor.Temperature);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(setting), setting.Color,
                    $"Unsupported colour type: {setting.Color.GetType()}");
        }
    }

    internal HueLightSetting GetSetting()
    {
        var on = Fields.OfType<LightOnSetupViewModel>().FirstOrDefault()?.On;
        var brightness = Fields.OfType<LightBrightnessSetupViewModel>().FirstOrDefault()?.GetBrightness();
        var color = Fields.OfType<LightColorSetupViewModelBase>().FirstOrDefault()?.GetHueColor();

        return new HueLightSetting(on, brightness, color);
    }
}