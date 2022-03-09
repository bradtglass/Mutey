namespace Mutey
{
    using Mutey.Input;

    public interface IMutey
    {
        void RegisterHardware( IMuteHardwareDetector detector );
        void RefreshHardware();
    }
}
