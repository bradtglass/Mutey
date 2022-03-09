namespace Mutey
{
    using Mutey.Input;
    using Prism.Ioc;
    using Prism.Modularity;

    public class MuteyRegistrationsModule : IModule
    {
        public void RegisterTypes( IContainerRegistry containerRegistry ) { }

        public void OnInitialized( IContainerProvider containerProvider )
        {
            var mutey = containerProvider.Resolve<IMutey>();
            mutey.RegisterHardware( new SerialHardwareDetector() );
            mutey.RefreshHardware();
        }
    }
}
