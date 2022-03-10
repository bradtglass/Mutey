namespace Mutey.Hardware.Serial.Ioc
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;

    [ ExcludeFromCodeCoverage ]
    public static class Registrations
    {
        public static IRegistrator RegisterSerialHardware( this IRegistrator registrator )
        {
            registrator.Register<IMuteHardwareDetector, SerialHardwareDetector>();

            return registrator;
        }
    }
}
