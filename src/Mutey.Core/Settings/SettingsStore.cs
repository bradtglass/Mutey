namespace Mutey.Core.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text.Json;

    public static class SettingsStore
    {
        private static readonly object IoLock = new();
        private static readonly Lazy<DirectoryInfo> SettingsDirectory = new(DirectoryFactory);
        private static readonly ConcurrentDictionary<Type, string> CachedFileNames = new();
        
        private static DirectoryInfo DirectoryFactory()
        {
            string localAppData = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
            string path = Path.Combine( localAppData, "Mutey", "settings" );

            return new DirectoryInfo( path );
        }

        private static string GetFilename<T>()
            where T : SettingsBase, new()
        {
            return CachedFileNames.GetOrAdd( typeof( T ), _ => new T().Filename );
        }

        private static FileInfo GetSubFile( string filename )
        {
            var directory = SettingsDirectory.Value;
            directory.Create();

            string path = Path.Combine( directory.FullName, filename );

            return new FileInfo( path );
        }

        public static T Get<T>()
            where T : SettingsBase, new()
        {
            lock ( IoLock )
            {
                string filename = GetFilename<T>();
                return GetSync<T>( filename, out _ );
            }
        }

        private static T GetSync<T>( string sub, out FileInfo file )
            where T : new()
        {
            file = GetSubFile( sub );

            if ( !file.Exists )
            {
                return new T();
            }

            using Stream stream = file.OpenRead();
            return JsonSerializer.Deserialize<T>( stream ) ?? new T();
        }

        public static T Set<T>( Func<T, T> update )
            where T : SettingsBase, new()
        {
            lock ( IoLock )
            {
                string fileName = GetFilename<T>();
                var initial = GetSync<T>( fileName, out var file );
                var updated = update( initial );

                using Stream stream = file.Create();
                JsonSerializer.Serialize( stream, updated );

                return updated;
            }
        }

        public static void Reset<T>()
            where T : SettingsBase, new()
        {
            lock ( IoLock )
            {
                string filename = GetFilename<T>();
                var file = GetSubFile( filename );

                if ( file.Exists )
                {
                    file.Delete();
                }
            }
        }

        public static void RegisterForNotifications<T>( Action<T> callback )
        {
            throw new NotImplementedException();
        }
    }
}
