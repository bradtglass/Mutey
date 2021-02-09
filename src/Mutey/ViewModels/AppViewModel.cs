using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Views;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class AppViewModel : BindableBase
    {
        private ImageSource? iconImage;

        public AppViewModel(MuteyViewModel mutey, Lazy<CompactView> lazyCompactView)
        {
            Mutey = mutey;
            QuitCommand = new ActionCommand(() => Application.Current.Shutdown());
            OpenCommand = new ActionCommand(() => lazyCompactView.Value.Show());
            ChangeIcon(default);
        }

        public ImageSource? IconImage
        {
            get => iconImage;
            internal set => SetProperty(ref iconImage, value);
        }

        public MuteyViewModel Mutey { get; }
        
        public ICommand QuitCommand { get; }
        public ICommand OpenCommand { get; }

        private void ChangeIcon(MicIconColours colour)
        {
            Uri uri = GetIconUri(colour);
            ImageSource source = new BitmapImage(uri);
            IconImage = source;
        }

        private static Uri GetIconUri(MicIconColours colour)
            => new($"pack://application:,,,/{typeof(AppViewModel).Assembly.GetName().Name};component/Icons/microphone-{colour.ToString().ToLower()}.ico");
        
        private enum MicIconColours
        {
            Black,
            Blue,
            Green,
            Red
        }
    }
}