namespace Mutey.Hue.Views
{
    using System.Windows;
    using Mutey.Hue.ViewModels.Setup;

    public partial class LightStateEditor
    {
        public static readonly DependencyProperty LightProperty = DependencyProperty.Register(
                                                                                              "Light", typeof( LightSetupViewModel ), typeof( LightStateEditor ),
                                                                                              new PropertyMetadata( default( LightSetupViewModel ) ) );

        public LightSetupViewModel Light
        {
            get => (LightSetupViewModel) GetValue( LightProperty );
            set => SetValue( LightProperty, value );
        }

        public LightStateEditor()
        {
            InitializeComponent();
        }
    }
}
