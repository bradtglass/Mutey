using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Mutey.Hue.Client;
using Mutey.Hue.Client.State;
using Mutey.Hue.Client.State.Color;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Mutey.Hue.ViewModels.Setup;

public class LightSetupViewModel : ObservableObject
{

    private Color color = Colors.Transparent;
    private bool resetFirst;
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

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public double MinTemp => 1e6 / (Info.Capabilities.Control.ColorTemperature?.Max ?? 500);

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public double MaxTemp => 1e6 / (Info.Capabilities.Control.ColorTemperature?.Min ?? 153);

    public Color Color
    {
        get => color;
        private set => SetProperty(ref color, value);
    }

    public bool ResetFirst
    {
        get => resetFirst;
        set => SetProperty(ref resetFirst, value);
    }

    private void RemoveField(LightFieldSetupViewModel? viewModel)
    {
        if (viewModel == null)
            return;

        Fields.Remove(viewModel);
        RefreshAvailable();
        RefreshColor();
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

        if (!Fields.OfType<LightOnSetupViewModel>().Any())
            AvailableFields.Add(LightField.On);

        if (!Fields.OfType<LightBrightnessSetupViewModel>().Any())
            AvailableFields.Add(LightField.Brightness);

        if (!Fields.OfType<LightColorSetupViewModelBase>().Any())
        {
            if (Info.Capabilities.Control.ColorGamut.HasValue)
                AvailableFields.Add(LightField.Color);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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

        foreach (var conflict in Fields.Where(f => f.ConflictsWith(field)).ToList()) 
            Fields.Remove(conflict);

        Fields.Add(field);
        
        if(field is IAffectsColor)
            field.PropertyChanged += OnColorComponentChanged;

        RefreshColor();
    }

    private void OnColorComponentChanged(object? sender, PropertyChangedEventArgs e)
        => RefreshColor();

    private void RefreshColor()
    {
        List<(byte? a, (byte r, byte g, byte b)?)> parts = Fields.OfType<IAffectsColor>()
            .Select(f => f.GetColorComponents())
            .ToList();

        var a = parts.Select(p => p.a).FirstOrDefault(a => a.HasValue) ?? byte.MaxValue;
        var (r, g, b) = parts.Select(p => p.Item2).FirstOrDefault(rgb => rgb.HasValue) ?? (255, 229, 207);

        Color = Color.FromArgb(a, r, g, b);
    }

    private void InitializeFields(HueLightSetting setting)
    {
        ResetFirst = setting.ResetFirst;
        
        if (setting.On is { } on)
            AddField<LightOnSetupViewModel>(vm => vm.On = on);

        if (setting.Brightness is { } brightness)
            AddField<LightBrightnessSetupViewModel>(vm => vm.SetBrightness(brightness));

        switch (setting.Color)
        {
            case null:
                break;
            case RgbHueColor rgbHueColor:
                var rgb = Color.FromRgb((byte)rgbHueColor.Color.R,
                    (byte)rgbHueColor.Color.G,
                    (byte)rgbHueColor.Color.B);
                AddField<LightColorSetupViewModel>(vm => vm.Color = rgb);

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
        var hueColor = Fields.OfType<LightColorSetupViewModelBase>().FirstOrDefault()?.GetHueColor();

        return new HueLightSetting(on, brightness, hueColor, ResetFirst);
    }
}