using HueMicIndicator.ViewModels;

namespace HueMicIndicator.Views;

public partial class SetupWindow
{
    public SetupWindow(HueSetupViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}