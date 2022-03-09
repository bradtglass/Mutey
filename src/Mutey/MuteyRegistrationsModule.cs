namespace Mutey
{
    using Mutey.Hardware;
    using Prism.Ioc;
    using Prism.Modularity;

    public class MuteyRegistrationsModule : IModule
    {
        public void RegisterTypes( IContainerRegistry containerRegistry ) { }

        public void OnInitialized( IContainerProvider containerProvider )
        {
            var mutey = containerProvider.Resolve<IMutey>();
            mutey.RegisterHardware( new SerialHardwareDetector() ); // TODO Move this registration logic in the hardware library
            mutey.RefreshHardware();
        }
    }
}
