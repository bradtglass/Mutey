using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueMicIndicator.Hue;
using HueMicIndicator.ViewModels.Setup;
using HueMicIndicator.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HueMicIndicator.ViewModels;

public sealed class ApplicationViewModel : ObservableObject, IDisposable, IInteractiveLoginHelper
{
    private readonly HueContext context = new();

    private bool isConfigured;

    public ApplicationViewModel()
    {
        State = new StateViewModel(context);

        RefreshIsConfigured();
        ConfigureCommand = new AsyncRelayCommand(ConfigureAsync);
        LaunchSetupCommand = new RelayCommand(LaunchSetup);
    }

    private void LaunchSetup()
    {
        SetupWindow window = new(new HueSetupViewModel(context));
        window.ShowDialog();
    }

    private void RefreshIsConfigured()
        => IsConfigured = context.IsConfigured();

    private async Task ConfigureAsync()
    {
        if(IsConfigured)
        {
            var result = MessageBox.Show("Would you like to reset the current configuration? This cannot be undone.", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            context.Reset();
            RefreshIsConfigured();
        }

        await context.LoginInteractiveAsync(this);
        RefreshIsConfigured();
        
        if(IsConfigured)
            LaunchSetup();
    }

    public StateViewModel State { get; }

    public ICommand ConfigureCommand { get; }
    
    public ICommand LaunchSetupCommand { get; }

    public bool IsConfigured
    {
        get => isConfigured;
        private set => SetProperty(ref isConfigured, value);
    }

    public void Dispose()
    {
        State.Dispose();
    }

    public Task<bool> RequestButtonPressAsync()
    {
        var result = MessageBox.Show("Press the button on your Hue hub and the press OK to continue connecting.", "Hue",
            MessageBoxButton.OKCancel);

        return Task.FromResult(result == MessageBoxResult.OK);
    }
}