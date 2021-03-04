using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Views;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    [UsedImplicitly]
    internal class AppViewModel : BindableBase
    {
        public AppViewModel(MuteyViewModel mutey, SettingsViewModel settings)
        {
            Mutey = mutey;
            Settings = settings;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            AboutCommand = new ActionCommand(() => new AboutWindow().Show());
            OpenCommand = new ActionCommand(() => new CompactWindow().Show());
        }

        public MuteyViewModel Mutey { get; }

        public ICommand QuitCommand { get; }
        
        public ICommand AboutCommand { get; }
        public ICommand OpenCommand { get; }

        public SettingsViewModel Settings { get; }
    }
}