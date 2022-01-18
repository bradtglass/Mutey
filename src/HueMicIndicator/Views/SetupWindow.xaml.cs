using HueMicIndicator.ViewModels.Setup;

namespace HueMicIndicator.Views;

public partial class SetupWindow
{
    public SetupWindow(HueSetupViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}