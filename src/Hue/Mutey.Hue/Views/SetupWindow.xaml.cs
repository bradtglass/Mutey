namespace Mutey.Hue.Views
{
    using Mutey.Hue.ViewModels.Setup;

    public partial class SetupWindow
    {
        public SetupWindow( HueSetupViewModel viewModel )
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.RequestClose += Close;
        }
    }
}
