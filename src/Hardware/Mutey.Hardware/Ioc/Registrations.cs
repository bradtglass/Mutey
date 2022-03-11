namespace Mutey.Hardware.Ioc
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;

    [ ExcludeFromCodeCoverage ]
    public static class Registrations
    {
        public static IRegistrator RegisterHardwareManagement( this IRegistrator registrator )
        {
            registrator.Register<IMuteDeviceManager, MuteDeviceManager>( Reuse.Singleton );

            registrator.RegisterInitializer<IMuteDeviceManager>( ( manager, context ) =>
                                                                   {
                                                                       var detectors = context.ResolveMany<IMuteDeviceDetector>();

                                                                       foreach ( var detector in detectors )
                                                                       {
                                                                           manager.RegisterDeviceDetector( detector );
                                                                       }
                                                                   } );
            return registrator;
        }
    }
}
