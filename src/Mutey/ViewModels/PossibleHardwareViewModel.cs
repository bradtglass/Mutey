using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class PossibleHardwareViewModel : BindableBase
    {
        private bool isActive;

        public PossibleHardwareViewModel(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public string Type { get; }

        public bool IsActive
        {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }
    }
}