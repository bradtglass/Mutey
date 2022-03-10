namespace Mutey.Core.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Text.Json;

    internal class SettingsStore : ISettingsStore
    {
        private readonly ConcurrentDictionary<Type, string> cachedFileNames = new();
        private readonly IFileSystem fileSystem;
        private readonly object ioLock = new();
        private readonly ConcurrentDictionary<Type, List<Action<SettingsChangedEventArgs>>> notificationRegistrations = new();
        private readonly Lazy<IDirectoryInfo> settingsDirectory;

        public SettingsStore( IFileSystem fileSystem )
        {
            this.fileSystem = fileSystem;
            settingsDirectory = new Lazy<IDirectoryInfo>( DirectoryFactory );
        }

        private IDirectoryInfo DirectoryFactory()
        {
            string localAppData = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
            string path = Path.Combine( localAppData, "Mutey", "settings" );

            return fileSystem.DirectoryInfo.FromDirectoryName( path );
        }

        private string GetFilename<T>()
            where T : SettingsBase, new()
        {
            return cachedFileNames.GetOrAdd( typeof( T ), _ => new T().Filename );
        }

        private IFileInfo GetFile<T>()
            where T : SettingsBase, new()
        {
            string filename = GetFilename<T>();

            var directory = settingsDirectory.Value;
            directory.Create();

            string path = Path.Combine( directory.FullName, filename );

            return fileSystem.FileInfo.FromFileName( path );
        }

        public T Get<T>()
            where T : SettingsBase, new()
        {
            lock ( ioLock )
            {
                return GetSync<T>( out _ );
            }
        }

        private T GetSync<T>( out IFileInfo file )
            where T : SettingsBase, new()
        {
            file = GetFile<T>();

            if ( !file.Exists )
            {
                return new T();
            }

            using var stream = file.OpenRead();
            return JsonSerializer.Deserialize<T>( stream ) ?? new T();
        }

        public T Set<T>( Func<T, T> update )
            where T : SettingsBase, new()
        {
            T updated;
            T initial;

            lock ( ioLock )
            {
                initial = GetSync<T>( out var file );
                updated = update( initial );

                using var stream = file.Create();
                JsonSerializer.Serialize( stream, updated );
            }

            NotifyChange( typeof( T ), initial, updated );

            return updated;
        }

        public void Reset<T>()
            where T : SettingsBase, new()
        {
            T oldValue;
            T newValue;

            lock ( ioLock )
            {
                var file = GetFile<T>();

                if ( !file.Exists )
                {
                    return;
                }

                oldValue = GetSync<T>( out _ );
                newValue = new T();

                file.Delete();
            }

            NotifyChange( typeof( T ), oldValue, newValue );
        }

        private void NotifyChange( Type settingType, object oldValue, object newValue )
        {
            var registrations = GetRegistrationList( settingType );

            if ( registrations.Count == 0 )
            {
                return;
            }

            var args = SettingsChangedEventArgs.Create( settingType, oldValue, newValue );

            foreach ( var registration in registrations )
            {
                registration( args );
            }
        }

        public void RegisterForNotifications<T>( Action<SettingsChangedEventArgs<T>> callback )
            where T : SettingsBase, new()
        {
            // BG There are some bad implications for memory leaks here which I'm not fixing right now but I'll try to come back to
            var registrationList = GetRegistrationList( typeof( T ) );
            registrationList.Add( o => callback( (SettingsChangedEventArgs<T>) o ) );
        }

        private List<Action<SettingsChangedEventArgs>> GetRegistrationList( Type settingType )
        {
            return notificationRegistrations.GetOrAdd( settingType, _ => new List<Action<SettingsChangedEventArgs>>() );
        }
    }
}
