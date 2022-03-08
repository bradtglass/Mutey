using System.Windows;
using HueMicIndicator.ViewModels.Setup;

namespace HueMicIndicator.Views;

public partial class LightStateEditor
{
    public static readonly DependencyProperty LightProperty = DependencyProperty.Register(
        "Light", typeof(LightSetupViewModel), typeof(LightStateEditor),
        new PropertyMetadata(default(LightSetupViewModel)));

    public LightStateEditor()
    {
        InitializeComponent();
    }

    public LightSetupViewModel Light
    {
        get => (LightSetupViewModel)GetValue(LightProperty);
        set => SetValue(LightProperty, value);
    }
}