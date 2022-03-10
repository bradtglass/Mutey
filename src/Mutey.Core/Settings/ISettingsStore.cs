namespace Mutey.Core.Settings
{
    using System;

    public interface ISettingsStore
    {
        T Get<T>()
            where T : SettingsBase, new();

        T Set<T>( Func<T, T> update )
            where T : SettingsBase, new();

        void Reset<T>()
            where T : SettingsBase, new();

        void RegisterForNotifications<T>( Action<SettingsChangedEventArgs<T>> callback )
            where T : SettingsBase, new();
    }
}
