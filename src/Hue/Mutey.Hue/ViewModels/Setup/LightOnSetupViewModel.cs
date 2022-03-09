namespace Mutey.Hue.ViewModels.Setup
{
    public class LightOnSetupViewModel : LightFieldSetupViewModel
    {
        private bool on;

        public bool On
        {
            get => on;
            set => SetProperty( ref on, value );
        }

        public LightOnSetupViewModel() : base( LightField.On ) { }
    }
}
