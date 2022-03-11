namespace Mutey.Core.Settings
{
    using System;

    public static class SettingsExtensions
    {
        /// <summary>
        ///     Register an callback to invoke when the settings of type <typeparamref name="T" /> are changed. This callback will
        ///     be invoked with the updated settings value.
        /// </summary>
        public static void RegisterForNotifications<T>( this ISettingsStore store, Action<T> callback )
            where T : SettingsBase, new()
        {
            store.RegisterForNotifications<T>( args => callback( args.NewValue ) );
        }
    }
}
