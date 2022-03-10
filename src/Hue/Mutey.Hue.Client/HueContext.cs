namespace Mutey.Hue.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using ComposableAsync;
    using Microsoft.Extensions.Caching.Memory;
    using Mutey.Core.Settings;
    using Mutey.Hue.Client.State;
    using Nito.AsyncEx;
    using Q42.HueApi;
    using Q42.HueApi.Interfaces;
    using RateLimiter;

    public class HueContext
    {
        private readonly ISettingsStore settingsStore;
        private readonly MemoryCache cache = new(new MemoryCacheOptions());
        private readonly AsyncLock loginLock = new();
        private readonly TimeLimiter rateLimiter = TimeLimiter.GetFromMaxCountByInterval( 10, TimeSpan.FromSeconds( 1 ) );
        private IHueClient? client;

        public HueStateStore StateStore { get; }

        public HueContext( ISettingsStore settingsStore )
        {
            this.settingsStore = settingsStore;
            StateStore = new HueStateStore( settingsStore, this );
        }

        public bool IsConfigured()
        {
            return GetSettings().AppKey != null;
        }

        private HueSettings GetSettings()
        {
            return settingsStore.Get<HueSettings>();
        }

        public void Reset()
        {
            settingsStore.Reset<HueSettings>();
        }

        private static async Task<string> GetBridgeIpAsync()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync( TimeSpan.FromSeconds( 30 ) );
            var bridge = bridges.First();

            return bridge.IpAddress;
        }

        public async Task LoginInteractiveAsync( IInteractiveLoginHelper loginHelper )
        {
            using var _ = await loginLock.LockAsync();
            if ( IsConfigured() )
            {
                try
                {
                    await LoginAsyncSync();

                    return;
                }
                catch ( Exception e )
                {
                    // TODO Log error
                    Console.WriteLine( e );
                }
            }

            string ip = await GetBridgeIpAsync();

            var loginClient = new LocalHueClient( ip );

            while ( true )
            {
                bool @continue = await loginHelper.RequestButtonPressAsync();

                if ( !@continue )
                {
                    return;
                }

                try
                {
                    string? appKey = await loginClient.RegisterAsync( "HueMicIndicator", Environment.MachineName );
                    if ( appKey == null )
                    {
                        continue;
                    }

                    settingsStore.Set<HueSettings>( s => s with {AppKey = appKey} );
                    await PostLoginAsyncSync( new LocalHueClient( ip, appKey ) );

                    return;
                }
                catch ( LinkButtonNotPressedException ) { }
            }
        }

        private async ValueTask PostLoginAsyncSync( IHueClient hueClient )
        {
            client = hueClient;
            await CacheAllLightsAsync();
        }

        private async ValueTask CacheAllLightsAsync()
        {
            var lights = await GetLightsAsyncCore();
            CacheLights( lights );
        }

        private void CacheLights( IEnumerable<Light> lights )
        {
            foreach ( var light in lights )
            {
                var setting = light.State.GetSetting();
                cache.Set( light.Id, setting );
            }
        }

        private async Task<IHueClient> LoginAsync()
        {
            using var _ = await loginLock.LockAsync();

            return await LoginAsyncSync();
        }

        public async Task<IHueClient> LoginAsyncSync()
        {
            if ( GetSettings().AppKey is not { } appKey )
            {
                throw new InvalidOperationException( "App key is not configured" );
            }

            string ip = await GetBridgeIpAsync();
            var innerClient = new LocalHueClient( ip, appKey );
            await PostLoginAsyncSync( innerClient );

            return innerClient;
        }

        private async ValueTask<T> ExecuteAsync<T>( Func<IHueClient, ValueTask<T>> callback )
        {
            if ( client is not { } innerClient )
            {
                innerClient = await LoginAsync();
            }

            await rateLimiter;

            return await callback( innerClient );
        }

        public async Task SetStateAsync( bool isActive )
        {
            var state = await StateStore.GetStateAsync( isActive );
            await ApplyStateAsync( state );
        }

        internal async ValueTask ApplyStateAsync( IHueState state, bool cacheState = true )
        {
            IEnumerable<Light> cachingLights;

            if ( cacheState )
            {
                // Get the initial state before applying the state
                IEnumerable<Light> allLights = ( await GetLightsAsyncCore() ).ToList();
                cachingLights = state.GetAffectedLights()
                                     .Select( al => allLights.FirstOrDefault( l => l.Id == al ) )
                                     .Where( l => l != null )!;
            }
            else
            {
                cachingLights = Array.Empty<Light>();
            }

            // Apply the state
            await state.ApplyAsync( this );

            if ( cacheState )
            {
                // Cache the initial values
                CacheLights( cachingLights );
            }
        }

        public async Task SendCommandAsync( LightCommand command, IEnumerable<string> lights )
        {
            await ExecuteAsync( async c => await c.SendCommandAsync( command, lights ) );
        }

        public async Task<IReadOnlyCollection<LightInfo>> GetLightsAsync()
        {
            const string cacheKey = "lights";

            if ( cache.Get<IReadOnlyCollection<LightInfo>>( cacheKey ) is { } result )
            {
                return result;
            }

            var lights = await GetLightsAsyncCore();
            var infoList = lights.Select( l => new LightInfo( l.Id, l.Name, l.Capabilities ) )
                                 .ToList();

            ReadOnlyCollection<LightInfo> infos = new(infoList);

            cache.Set( cacheKey, infos, TimeSpan.FromHours( 1 ) );

            return infos;
        }

        private async Task<IEnumerable<Light>> GetLightsAsyncCore()
        {
            return await ExecuteAsync( async c => await c.GetLightsAsync() );
        }

        public async ValueTask<StateReset> PreviewStateAsync( IHueState state )
        {
            // Enumerate affected lights and store current state
            var currentLights = ( await GetLightsAsyncCore() ).ToList();
            var lightStates = state.GetAffectedLights()
                                   .Distinct()
                                   .Select( id => currentLights.FirstOrDefault( l => l.Id.Equals( id ) ) )
                                   .Where( l => l != null )
                                   .Select( l => ( l!.Id, Setting : l.State.GetSetting() ) )
                                   .GroupBy( l => l.Item2 )
                                   .Select( l => new HueLightState( l.Key, l.Select( ll => ll.Id ).ToList() ) )
                                   .ToList();
            var resetState = HueState.Get( lightStates );

            // Apply new state
            await ApplyStateAsync( state );

            // Return reset object
            return new StateReset( resetState, this, false /*Don't cache the previewed state*/ );
        }

        public async ValueTask<string?> FindLightIdAsync( string name )
        {
            var lights = await GetLightsAsync();

            return lights.FirstOrDefault( l
                                              => l.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ||
                                                 l.Id.Equals( name, StringComparison.OrdinalIgnoreCase ) )
                        ?.Id;
        }

        public HueLightSetting? GetLastLightState( string id )
        {
            return cache.Get<HueLightSetting>( id );
        }
    }
}
