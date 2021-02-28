using Mutey.Input;

namespace Mutey
{
    public interface IMutey
    {
        void RegisterHardware(IMuteHardwareDetector detector);
        void RefreshHardware();
    }
}