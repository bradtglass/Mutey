﻿namespace Mutey.Hue.Client.State
{
    using System.Threading.Tasks;
    using Mutey.Core.Settings;

    public class HueStateStore
    {
        private readonly ISettingsStore settingsStore;
        private readonly HueContext context;

        internal HueStateStore( ISettingsStore settingsStore, HueContext context )
        {
            this.settingsStore = settingsStore;
            this.context = context;
        }

        public HueStateSetting Get( bool isActive )
        {
            string key = GetStateKey( isActive );
            var settings = settingsStore.Get<HueSettings>().States;
            if ( settings.TryGetValue( key, out var setting ) )
            {
                return setting;
            }

            return HueStateSetting.Empty;
        }

        public async ValueTask<IHueState> GetStateAsync( bool isActive )
        {
            var setting = Get( isActive );

            return await HueState.CreateAsync( setting, context );
        }

        public void Set( bool isActive, HueStateSetting setting )
        {
            string key = GetStateKey( isActive );
            settingsStore.Set<HueSettings>( s => s with
            {
                States = s.States.SetItem( key, setting )
            } );
        }

        private static string GetStateKey( bool isActive )
        {
            return isActive ? "Active" : "Inactive";
        }
    }
}
