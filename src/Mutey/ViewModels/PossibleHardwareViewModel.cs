using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class PossibleHardwareViewModel : BindableBase
    {
        private bool isActive;

        public PossibleHardwareViewModel(string name, string type, string id)
        {
            Name = name;
            Type = type;
            Id = id;
        }

        public string Name { get; }

        public string Type { get; }
        
        public string Id { get; }

        public bool IsActive
        {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }
    }
}