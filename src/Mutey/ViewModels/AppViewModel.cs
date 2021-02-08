using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Views;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class AppViewModel : BindableBase
    {
        private ImageSource? iconImage;

        public AppViewModel(MuteyViewModel mutey)
        {
            Mutey = mutey;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            OpenCommand = new ActionCommand(() => new CompactView().Show());
            SetImage();
        }

        public ImageSource? IconImage
        {
            get => iconImage;
            internal set => SetProperty(ref iconImage, value);
        }

        private void SetImage()
        {
            return;
        }
        
        public MuteyViewModel Mutey { get; }
        
        public ICommand QuitCommand { get; }
        public ICommand OpenCommand { get; }
    }
}