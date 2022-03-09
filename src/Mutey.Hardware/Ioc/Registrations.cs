namespace Mutey.Hardware.Ioc
{
    using DryIoc;

    public static class Registrations
    {
        public static IRegistrator RegisterHardwareManagement( this IRegistrator registrator )
        {
            registrator.Register<IMuteHardwareManager, MuteHardwareManager>( Reuse.Singleton );

            return registrator;
        }
    }
}
