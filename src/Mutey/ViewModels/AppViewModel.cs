using System;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Views;
using NLog;
using Prism.Mvvm;
using Squirrel;

namespace Mutey.ViewModels
{
    [UsedImplicitly]
    internal class AppViewModel : BindableBase
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public AppViewModel(MuteyViewModel mutey, SettingsViewModel settings)
        {
            Mutey = mutey;
            Settings = settings;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            AboutCommand = new ActionCommand(() => new AboutWindow().Show());
            OpenCommand = new ActionCommand(() => new SettingsWindow().Show());
            RunUserInvokedUpdateCheckCommand = new ActionCommand(RunUserInvokedUpdateCheck);
        }

        private static async void RunUserInvokedUpdateCheck()
        {
            try
            {
                logger.Info("Beginning update check");

                using IUpdateManager updateManager = await Task.Run(()=> UpdateManager.GitHubUpdateManager(@"https://github.com/G18SSY/Mutey"));
                UpdateInfo updateInfo = await Task.Run(() => updateManager.CheckForUpdate());

                if (updateInfo.CurrentlyInstalledVersion.Version == updateInfo.FutureReleaseEntry.Version)
                {
                    logger.Info("Update check determined latest version is current version");
                    MessageBox.Show($"Already running latest version ({updateInfo.CurrentlyInstalledVersion.Version})",
                        "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    return;
                }
                
                logger.Info("Latest version is {Version}", updateInfo.FutureReleaseEntry.Version);
                MessageBoxResult result = MessageBox.Show(
                    $"Version {updateInfo.FutureReleaseEntry.Version} is available, would you like to download and install?",
                    "Update", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    logger.Info("User chose not to install update");
                    
                    return;
                }

                await updateManager.DownloadReleases(updateInfo.ReleasesToApply);
                await updateManager.ApplyReleases(updateInfo);
                await updateManager.CreateUninstallerRegistryEntry();

                logger.Info("Update applied");
                
                result = MessageBox.Show("Update applied successfully, would you like to restart?", "Update",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    logger.Info("Restarting application");
                    UpdateManager.RestartApp();
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error durign update check");
            }
            AboutCommand = new ActionCommand(() => new AboutWindow().Show());
            OpenCommand = new ActionCommand(() => new SettingsWindow().Show());
        }

        public MuteyViewModel Mutey { get; }

        public ICommand QuitCommand { get; }
        
        public ICommand AboutCommand { get; }
        
        public ICommand OpenCommand { get; }
        
        public ICommand RunUserInvokedUpdateCheckCommand { get; }
        
        public SettingsViewModel Settings { get; }
    }
}