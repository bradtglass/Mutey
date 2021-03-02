using System.Diagnostics;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Mutey.Input;
using Mutey.Mute;
using Mutey.ViewModels;
using Mutey.Views;
using NLog;
using NLog.Config;
using NLog.Targets;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            LoggingConfiguration current = LogManager.Configuration??
                                           new LoggingConfiguration();
            
            ConfigureSentinelLogging(current);

            LogManager.Configuration = current;
            
            logger.Info("Beginning startup");
            
            base.OnStartup(e);
        }

        [Conditional("DEBUG")]
        private static void ConfigureSentinelLogging(LoggingConfiguration configuration)
        {
            NLogViewerTarget target = new("Sentinel")
            {
                Address = "udp://127.0.0.1:9999"
            };

            configuration.AddRule(LogLevel.Trace,LogLevel.Fatal, target);
        }
        
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
            containerRegistry.RegisterSingleton<ISystemMuteControl, SystemMuteControl>();
            containerRegistry.RegisterSingleton<AppViewModel>();
            containerRegistry.RegisterManySingleton<MuteyViewModel>(typeof(MuteyViewModel), typeof(IMutey));
            containerRegistry.RegisterSingleton<CompactView>();
            containerRegistry.RegisterSingleton<AboutWindow>();
        }

        protected override Window? CreateShell()
            => null;
    }
}