namespace Mutey
{
    using Mutey.Hardware;

    public interface IMutey
    {
        void RegisterHardware( IMuteHardwareDetector detector );
        void RefreshHardware();
    }
}
