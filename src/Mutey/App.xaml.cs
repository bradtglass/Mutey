using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Mutey.ViewModels;
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
            => new ModuleCatalog(new[] {new ModuleInfo(typeof(TaskBarModule))});

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<TaskbarIcon, AppViewModel>();
        }

        protected override Window? CreateShell()
            => null;
    }

    public class TaskBarModule : IModule
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void RegisterTypes(IContainerRegistry containerRegistry) { }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            logger.Info("Finding TaskbarIcon resource");
            TaskbarIcon taskbarIcon = (TaskbarIcon) Application.Current.Resources["TaskbarIcon"];

            logger.Info("Configuring view model for TaskbarIcon");
            AppViewModel appViewModel = containerProvider.Resolve<AppViewModel>();
            taskbarIcon.DataContext = appViewModel;
        }
    }
}