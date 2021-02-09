using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Mutey.Input;
using Mutey.ViewModels;
using Mutey.Views;
using NLog;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace Mutey
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected override IModuleCatalog CreateModuleCatalog()
            => new ModuleCatalog(new[]
            {
                new ModuleInfo(typeof(TaskBarModule)),
                new ModuleInfo(typeof(MuteyRegistrationsModule))
            });

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            logger.Info("Registering types with Prism");
            
            ViewModelLocationProvider.Register<TaskbarIcon, AppViewModel>();
            ViewModelLocationProvider.Register<CompactView, AppViewModel>();

            containerRegistry.RegisterSingleton<IMuteHardwareManager, MuteHardwareManager>();
            containerRegistry.RegisterSingleton<AppViewModel>();
            containerRegistry.RegisterManySingleton<MuteyViewModel>(typeof(MuteyViewModel), typeof(IMutey));
            containerRegistry.RegisterSingleton<CompactView>();
            containerRegistry.RegisterSingleton<AboutWindow>();
        }

        protected override Window? CreateShell()
            => null;
    }
}