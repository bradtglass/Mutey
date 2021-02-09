using System;
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
        public AppViewModel(MuteyViewModel mutey, Lazy<CompactView> lazyCompactView, Lazy<AboutWindow> lazyAboutView)
        {
            Mutey = mutey;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            AboutCommand = new ActionCommand(() => lazyAboutView.Value.Show());
            OpenCommand = new ActionCommand(() => lazyCompactView.Value.Show());
        }

        public MuteyViewModel Mutey { get; }

        public ICommand QuitCommand { get; }
        
        public ICommand AboutCommand { get; }
        public ICommand OpenCommand { get; }
    }
}