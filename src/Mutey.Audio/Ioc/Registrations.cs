namespace Mutey.Audio.Ioc
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;
    using Mutey.Audio.Mute;

    [ ExcludeFromCodeCoverage ]
    public static class Registrations
    {
        public static IRegistrator RegisterAudioManagement( this IRegistrator registrator )
        {
            registrator.Register<ISystemMuteControl, SystemMuteControl>( Reuse.Singleton );

            return registrator;
        }
    }
}
