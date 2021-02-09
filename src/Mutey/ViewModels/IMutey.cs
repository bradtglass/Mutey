using Mutey.Input;
using Mutey.Output;

namespace Mutey.ViewModels
{
    public interface IMutey
    {
        void RegisterApp(IConferencingAppRegistration registration);
        void RegisterHardware(IMuteHardwareDetector detector);
        void FullRefresh();
    }
}