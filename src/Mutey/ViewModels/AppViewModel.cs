using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Views;
using NLog;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    [UsedImplicitly]
    internal class AppViewModel : BindableBase
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IUpdateService updateService;

        private UpdateState updateState;

        public AppViewModel(MuteyViewModel mutey, SettingsViewModel settings, IUpdateService updateService)
        {
            this.updateService = updateService;
            updateService.StateChanged += OnUpdateStateChanged;
            UpdateState = updateService.State;

            Mutey = mutey;
            Settings = settings;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            AboutCommand = new ActionCommand(() => new AboutWindow().Show());
            OpenCommand = new ActionCommand(() => new SettingsWindow().Show());
            UpdateCommand = new ActionCommand(RequestUpdate);
            CheckForUpdateCommand = new ActionCommand(CheckForUpdate);
        }

        public UpdateState UpdateState
        {
            get => updateState;
            private set => SetProperty(ref updateState, value);
        }

        public MuteyViewModel Mutey { get; }

        public ICommand QuitCommand { get; }

        public ICommand AboutCommand { get; }

        public ICommand OpenCommand { get; }

        public ICommand UpdateCommand { get; }

        public ICommand CheckForUpdateCommand { get; }

        public SettingsViewModel Settings { get; }

        private async void CheckForUpdate()
        {
            try
            {
                UpdateState state = await Task.Run(updateService.RefreshStateAsync);
                if (state != UpdateState.Available)
                {
                    MessageBox.Show("No updates available", "Up to date", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    "An update is available, would you like to download and install?",
                    "Update", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                await Task.Run(updateService.UpdateAsync);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error whilst checking for updates");
            }
        }

        private void OnUpdateStateChanged(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateState = updateService.State);
        }

        private async void RequestUpdate()
        {
            try
            {
                if (updateService.State == UpdateState.Available)
                    await Task.Run(updateService.UpdateAsync);
                else
                    CheckForUpdate();
            }
            catch (Exception e)
            {
                logger.Error(e, "Error during update check");
            }
        }
    }
}