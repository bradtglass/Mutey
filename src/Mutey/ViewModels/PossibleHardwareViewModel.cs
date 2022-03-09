namespace Mutey.ViewModels
{
    using Prism.Mvvm;

    internal class PossibleHardwareViewModel : BindableBase
    {
        private bool isActive;

        public string Name { get; }

        public string Type { get; }

        public string Id { get; }

        public bool IsActive
        {
            get => isActive;
            set => SetProperty( ref isActive, value );
        }

        public PossibleHardwareViewModel( string name, string type, string id )
        {
            Name = name;
            Type = type;
            Id = id;
        }
    }
}
