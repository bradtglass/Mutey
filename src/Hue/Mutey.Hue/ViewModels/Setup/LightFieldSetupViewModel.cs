namespace Mutey.Hue.ViewModels.Setup
{
    using Humanizer;
    using Microsoft.Toolkit.Mvvm.ComponentModel;

    public abstract class LightFieldSetupViewModel : ObservableObject
    {
        public LightField Field { get; }

        public string FieldName { get; }

        protected LightFieldSetupViewModel( LightField field )
        {
            Field = field;
            FieldName = field.Humanize( LetterCasing.Title );
        }

        public virtual bool ConflictsWith( LightFieldSetupViewModel other )
        {
            return false;
        }
    }
}
