using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueMicIndicator.Hue;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HueMicIndicator.ViewModels;

public sealed class ApplicationViewModel : ObservableObject, IDisposable, IInteractiveLoginHelper
{
    private readonly HueHandler hueHandler = new();

    private bool isConfigured;

    public ApplicationViewModel()
    {
        State = new StateViewModel(hueHandler);

        RefreshIsConfigured();
        ConfigureCommand = new AsyncRelayCommand(ConfigureAsync);
    }

    private void RefreshIsConfigured()
        => IsConfigured = hueHandler.IsConfigured();

    private async Task ConfigureAsync()
    {
        if(IsConfigured)
        {
            var result = MessageBox.Show("Would you like to reset the current configuration? This cannot be undone.", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            RefreshIsConfigured();
        }

        await hueHandler.LoginInteractiveAsync(this);
        RefreshIsConfigured();
    }

    public StateViewModel State { get; }

    public ICommand ConfigureCommand { get; }

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