namespace Mutey.Audio.Ioc
{
    using DryIoc;
    using Mutey.Audio.Mute;

    public static class Registrations
    {
        public static IRegistrator RegisterAudioManagement( this IRegistrator registrator )
        {
            registrator.Register<ISystemMuteControl, SystemMuteControl>( Reuse.Singleton );

            return registrator;
        }
    }
}
