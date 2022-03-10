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

            registrator.RegisterInitializer<IMuteHardwareManager>( ( manager, context ) =>
                                                                   {
                                                                       var detectors = context.ResolveMany<IMuteHardwareDetector>();

                                                                       foreach ( var detector in detectors )
                                                                       {
                                                                           manager.RegisterHardwareDetector( detector );
                                                                       }
                                                                   } );
            return registrator;
        }
    }
}
