using Humanizer;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels.Setup;

public abstract class LightFieldSetupViewModel : ObservableObject
{
    protected LightFieldSetupViewModel(LightField field)
    {
        Field = field;
        FieldName = field.Humanize(LetterCasing.Title);
    }

    public LightField Field { get; }

    public string FieldName { get; }

    public virtual bool ConflictsWith(LightFieldSetupViewModel other)
        => false;
}