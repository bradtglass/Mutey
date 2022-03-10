namespace Mutey.Hardware.Ioc
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;

    [ ExcludeFromCodeCoverage ]
    public static class Registrations
    {
        public static IRegistrator RegisterHardwareManagement( this IRegistrator registrator )
        {
            registrator.Register<IMuteHardwareManager, MuteHardwareManager>( Reuse.Singleton );

            return registrator;
        }
    }
}
