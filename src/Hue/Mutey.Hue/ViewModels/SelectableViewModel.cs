namespace Mutey.Hue.ViewModels
{
    using Microsoft.Toolkit.Mvvm.ComponentModel;

    public class SelectableViewModel<T> : ObservableObject
    {
        private bool isSelected;

        public T Value { get; }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty( ref isSelected, value );
        }

        public SelectableViewModel( T value )
        {
            Value = value;
        }
    }
}
