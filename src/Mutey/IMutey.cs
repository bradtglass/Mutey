using Mutey.Input;
using Mutey.Output;

namespace Mutey
{
    public interface IMutey
    {
        void RegisterApp(IConferencingAppRegistration registration);
        void RegisterHardware(IMuteHardwareDetector detector);
        void FullRefresh();
    }
}