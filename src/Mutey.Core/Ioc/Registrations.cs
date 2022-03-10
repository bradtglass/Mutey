namespace Mutey.Core.Ioc
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using DryIoc;
    using Mutey.Core.Audio;
    using Mutey.Core.Settings;

    [ ExcludeFromCodeCoverage ]
    public static class Registrations
    {
        public static IRegistrator RegisterSettingsStore( this IRegistrator registrator )
        {
            registrator.Register<IFileSystem, FileSystem>( Reuse.Singleton );
            registrator.Register<ISettingsStore, SettingsStore>( Reuse.Singleton );

            return registrator;
        }

        public static IRegistrator RegisterAudioManagement( this IRegistrator registrator )
        {
            registrator.Register<ISystemMuteControl, SystemMuteControl>( Reuse.Singleton );

            return registrator;
        }
    }
}
