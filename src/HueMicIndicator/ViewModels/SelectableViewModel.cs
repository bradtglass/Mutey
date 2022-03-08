using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels;

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