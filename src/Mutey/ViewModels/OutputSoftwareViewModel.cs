using System.Windows.Input;
using System.Windows.Media;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class OutputSoftwareViewModel : BindableBase
    {
        private bool isConnected;

        public OutputSoftwareViewModel(string name, ICommand refreshCommand, ImageSource? icon)
        {
            Name = name;
            RefreshCommand = refreshCommand;
            Icon = icon;
        }

        public bool IsConnected
        {
            get => isConnected;
            set => SetProperty(ref isConnected, value);
        }

        public string Name { get; }

        public ICommand RefreshCommand { get; }
        
        public ImageSource? Icon { get; }
    }
}