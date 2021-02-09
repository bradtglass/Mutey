using Mutey.Input;
using Mutey.Output;
using Prism.Ioc;
using Prism.Modularity;

namespace Mutey
{
    public class MuteyRegistrationsModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry) { }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            IMutey mutey = containerProvider.Resolve<IMutey>();
            mutey.RegisterApp(new DiscordApp());
            mutey.RegisterHardware(new SerialHardwareDetector());
            mutey.FullRefresh();
        }
    }
}