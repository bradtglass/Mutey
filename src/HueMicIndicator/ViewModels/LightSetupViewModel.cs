using System.Windows.Media;
using HueMicIndicator.Hue;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels;

public class LightSetupViewModel : ObservableObject
{
    private double? brightness;
    private Color? color;

    private bool? on;

    public LightSetupViewModel(LightInfo lightInfo)
    {
        Info = lightInfo;
    }

    public LightInfo Info { get; }

    public bool? On
    {
        get => on;
        set => SetProperty(ref on, value);
    }

    public Color? Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }

    public double? Brightness
    {
        get => brightness;
        set => SetProperty(ref brightness, value);
    }
}