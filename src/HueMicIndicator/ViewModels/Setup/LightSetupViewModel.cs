using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using HueMicIndicator.Hue;
using HueMicIndicator.Hue.State;
using HueMicIndicator.Hue.State.Color;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Q42.HueApi.ColorConverters;

namespace HueMicIndicator.ViewModels.Setup;

public class LightSetupViewModel : ObservableObject
{
    public LightSetupViewModel(LightInfo lightInfo, HueLightSetting? initialSetting)
    {
        Info = lightInfo;
        if (initialSetting != null)
            InitializeFields(initialSetting);
        RefreshAvailable();
    }

    public LightInfo Info { get; }

    public ObservableCollection<LightField> AvailableFields { get; } = new();

    public LightField? NextField
    {
        get => null;
        set
        {
            if(value.HasValue)
                AddField(value.Value);
        }
    }

    private void RemoveField(LightFieldSetupViewModel viewModel)
    {
        Fields.Remove(viewModel);
        RefreshAvailable();
    }

    private void AddField(LightField field)
    {
        Fields.Add(GetField(field));
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

    public ObservableCollection<LightFieldSetupViewModel> Fields { get; } = new();

    internal void RefreshAvailable()
    {
        AvailableFields.Clear();

        if (!Fields.Any(f => f is LightOnSetupViewModel))
            AvailableFields.Add(LightField.On);

        if (!Fields.Any(f => f is LightBrightnessSetupViewModel))
            AvailableFields.Add(LightField.Brightness);

        if (!Fields.Any(f => f is LightColorSetupViewModel or LightColorTempSetupViewModel))
        {
            if (Info.Capabilities.Control.ColorGamut.HasValue)
                AvailableFields.Add(LightField.Color);

            AvailableFields.Add(LightField.ColorTemperature);
        }
    }

    private void InitializeFields(HueLightSetting setting)
    {
        viewModel.On = setting.On;
        viewModel.Brightness = setting.Brightness.HasValue ? (double)setting.Brightness.Value / byte.MaxValue : null;

        switch (setting.Color)
        {
            case null:
                viewModel.Color = null;
                break;
            case RgbHueColor rgbHueColor:
                viewModel.Color = Color.FromRgb((byte)rgbHueColor.Color.R,
                    (byte)rgbHueColor.Color.G,
                    (byte)rgbHueColor.Color.B);
                break;
            // TODO
            // case TemperatureHueColor temperatureHueColor:
            //     break;
            // case XyHueColor xyHueColor:
            //     break;
            default:
                throw new ArgumentOutOfRangeException(nameof(setting), setting.Color,
                    $"Unsupported colour type: {setting.Color.GetType()}");
        }
    }

    internal HueLightSetting GetSetting()
    {
        RgbHueColor? color = null;
        if(Fields.)

        if (viewModel.Color is { } vmColor)
            color = new RgbHueColor(new RGBColor(vmColor.R, vmColor.G, vmColor.B));

        return new HueLightSetting(viewModel.On, null, color);
    }
}