using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Mutey.ViewModels;
using NLog;
using Prism.Ioc;
using Prism.Modularity;

namespace Mutey
{
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