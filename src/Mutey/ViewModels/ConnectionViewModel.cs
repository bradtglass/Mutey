namespace Mutey.ViewModels
{
    using System.Windows.Input;
    using System.Windows.Media;
    using Prism.Mvvm;

    internal class ConnectionViewModel : BindableBase
    {
        private bool isConnected;

        public bool IsConnected
        {
            get => isConnected;
            set => SetProperty( ref isConnected, value );
        }

        public string Name { get; }

        public ICommand RefreshCommand { get; }

        public ImageSource? Icon { get; }

        public ConnectionViewModel( string name, ICommand refreshCommand, ImageSource? icon )
        {
            Name = name;
            RefreshCommand = refreshCommand;
            Icon = icon;
        }
    }
}
