namespace Mutey.Core.Settings
{
    using System;
    using System.Reflection;

    public class SettingsChangedEventArgs : EventArgs
    {
        internal static SettingsChangedEventArgs Create( Type settingType, object oldValue, object newValue )
        {
            var type = typeof( SettingsChangedEventArgs );
            var method = type.GetMethod( nameof( CreateCore ), BindingFlags.Static | BindingFlags.NonPublic ) ??
                         throw new InvalidOperationException( "Method not found with reflection" );

            method = method.MakeGenericMethod( settingType );

            var args = (SettingsChangedEventArgs) method.Invoke( null, new[] {oldValue, newValue} )!;

            return args;
        }

        private static SettingsChangedEventArgs<T> CreateCore<T>( T oldValue, T newValue )
        {
            return new(oldValue, newValue);
        }
    }

    public class SettingsChangedEventArgs<T> : SettingsChangedEventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }

        internal SettingsChangedEventArgs( T oldValue, T newValue )
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
