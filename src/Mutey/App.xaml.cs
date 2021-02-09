using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Mutey.Input;
using Mutey.ViewModels;
using Mutey.Views;
using NLog;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace Mutey
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected override IModuleCatalog CreateModuleCatalog()
            => new ModuleCatalog(new[]
            {
                new ModuleInfo(typeof(TaskBarModule)),
                new ModuleInfo(typeof(OutputRegistrationsModule))
            });

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<TaskbarIcon, AppViewModel>();
            ViewModelLocationProvider.Register<CompactView, AppViewModel>();

            containerRegistry.RegisterSingleton<IMuteHardwareManager, MuteHardwareManager>();
            containerRegistry.RegisterSingleton<AppViewModel>();
            containerRegistry.RegisterManySingleton<MuteyViewModel>(typeof(MuteyViewModel), typeof(IMutey));
            containerRegistry.RegisterSingleton<CompactView>();
        }

        protected override Window? CreateShell()
            => null;
    }
}