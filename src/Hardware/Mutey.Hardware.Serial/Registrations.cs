namespace Mutey.Hardware.Serial
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
            registrator.Register<IMuteDeviceDetector, SerialDeviceDetector>();
        }
    }
}
