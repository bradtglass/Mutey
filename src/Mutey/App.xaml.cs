namespace Mutey
{
    using System.Diagnostics;
    using System.Windows;
    using Hardcodet.Wpf.TaskbarNotification;
    using Mutey.Audio.Ioc;
    using Mutey.Core.Ioc;
    using Mutey.Hardware.Ioc;
    using Mutey.Popup;
    using Mutey.ViewModels;
    using Mutey.Views;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Prism.DryIoc;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup( StartupEventArgs e )
        {
            var current = LogManager.Configuration ??
                          new LoggingConfiguration();

            ConfigureFileLogging( current );
            ConfigureSentinelLogging( current );

            LogManager.Configuration = current;

            logger.Info( "Beginning startup" );

            base.OnStartup( e );
        }

        [ Conditional( "DEBUG" ) ]
        private static void ConfigureSentinelLogging( LoggingConfiguration configuration )
        {
            NLogViewerTarget target = new("Sentinel")
            {
                Address = "udp://127.0.0.1:9999"
            };

            configuration.AddRule( LogLevel.Trace, LogLevel.Fatal, target );
        }

        private static void ConfigureFileLogging( LoggingConfiguration configuration )
        {
            FileTarget target = new("File")
            {
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveDays = 3,
                FileName = "${specialfolder:folder=localapplicationdata}/Mutey/logs/mutey.log",
                Layout = "${longdate} - ${level:uppercase=true}: ${message}${onexception:${newline}exception\\: ${exception:format=tostring}}"
            };

            var minLevel = Debugger.IsAttached ? LogLevel.Debug : LogLevel.Info;

            configuration.AddRule( minLevel, LogLevel.Fatal, target );
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog( new[]
            {
                new ModuleInfo( typeof( TaskBarModule ) ),
                new ModuleInfo( typeof( MuteyRegistrationsModule ) )
            } );
        }

        protected override void RegisterTypes( IContainerRegistry containerRegistry )
        {
            logger.Info( "Registering types with Prism" );

            ViewModelLocationProvider.Register<TaskbarIcon, AppViewModel>();
            ViewModelLocationProvider.Register<SettingsWindow, AppViewModel>();

            containerRegistry.GetContainer()
                             .RegisterSettingsStore()
                             .RegisterHardwareManagement()
                             .RegisterAudioManagement();
            
            containerRegistry.RegisterSingleton<AppViewModel>();
            containerRegistry.RegisterManySingleton<MuteyViewModel>( typeof( MuteyViewModel ), typeof( IMutey ) );
            containerRegistry.RegisterSingleton<SettingsWindow>();
            containerRegistry.RegisterSingleton<AboutWindow>();
            containerRegistry.RegisterSingleton<MicStatePopupManager>();
            containerRegistry.RegisterSingleton<IUpdateService, UpdateService>();
        }

        protected override Window? CreateShell()
        {
            return null;
        }
    }
}
