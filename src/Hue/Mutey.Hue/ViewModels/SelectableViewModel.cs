using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Mutey.Hue.ViewModels;

public class SelectableViewModel<T> : ObservableObject
{
    private bool isSelected;

    public SelectableViewModel(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }
}