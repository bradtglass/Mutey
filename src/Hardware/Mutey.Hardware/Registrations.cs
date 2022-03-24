namespace Mutey.Hardware
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;
    using JetBrains.Annotations;
    using Mutey.Core;

    [ ExcludeFromCodeCoverage ]
    [ UsedImplicitly ]
    public class Registrations : IMuteyRegistrator
    {
        public void Register( IRegistrator registrator )
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
        }
    }
}
