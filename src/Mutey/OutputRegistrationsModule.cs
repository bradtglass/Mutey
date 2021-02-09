using Mutey.Output;
using Mutey.ViewModels;
using Prism.Ioc;
using Prism.Modularity;

namespace Mutey
{
    public class OutputRegistrationsModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry) { }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            IMutey mutey = containerProvider.Resolve<IMutey>();
            mutey.RegisterApp(new DiscordApp());
            mutey.FullRefresh();
        }
    }
}