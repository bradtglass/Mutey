using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HueMicIndicator.Core.Settings;
using Microsoft.Extensions.Caching.Memory;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace HueMicIndicator.Hue
{
    public class HueContext
    {
        private readonly MemoryCache cache = new(new MemoryCacheOptions());
        public HueStateStore StateStore { get; }
        private IHueClient? client;
        public HueContext()
        {
            StateStore = new HueStateStore(this);
        }

        public bool IsConfigured()
            => GetSettings().AppKey != null;

        private static HueSettings GetSettings()
            => SettingsStore.Get<HueSettings>(HueSettings.Sub);
        
        public void Reset()
            => SettingsStore.Reset(HueSettings.Sub);

        private static async Task<string> GetBridgeIpAsync()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(30));
            var bridge = bridges.First();

            return bridge.IpAddress;
        }
        
        public async Task LoginInteractiveAsync(IInteractiveLoginHelper loginHelper)
        {
            if (IsConfigured())
            {
                try
                {
                    await LoginAsync();
                    
                    return;
                }
                catch (Exception e)
                {
                    // TODO Log error
                    Console.WriteLine(e);
                }
            }

            var ip = await GetBridgeIpAsync();

            var loginClient = new LocalHueClient(ip);
            
            while (true)
            {
                bool @continue = await loginHelper.RequestButtonPressAsync();

                if (!@continue)
                    return;
                
                try
                {
                    var appKey = await loginClient.RegisterAsync("HueMicIndicator", Environment.MachineName);

                    SettingsStore.Set<HueSettings>(HueSettings.Sub, s => s with { AppKey = appKey });
                    
                    return;
                }
                catch (LinkButtonNotPressedException) { }
            }
        }
        
        public async Task<IHueClient> LoginAsync()
        {
            if (GetSettings().AppKey is not { } appKey)
                throw new InvalidOperationException("App key is not configured");
            
            var ip = await GetBridgeIpAsync();
            var innerClient = new LocalHueClient(ip, appKey);
            client = innerClient;

            return innerClient;
        }

        public async Task ChangeState(bool isActive)
        {
            if (client is not { } innerClient)
                innerClient = await LoginAsync();

            var state = await StateStore.GetAsync(isActive);
            await state.ApplyAsync(innerClient);
        }

        public async Task<IReadOnlyCollection<LightInfo>> GetLightsAsync()
        {
            const string cacheKey = "lights";

            if (cache.Get<IReadOnlyCollection<LightInfo>>(cacheKey) is { } result)
                return result;

            if (client is not { } innerClient)
                innerClient = await LoginAsync();

            IEnumerable<Light> lights = await innerClient.GetLightsAsync();
            List<LightInfo> infoList = lights.Select(l=>new LightInfo(l.Id, l.Name, l.Capabilities)).ToList();
            var infos = new ReadOnlyCollection<LightInfo>(infoList);

            cache.Set(cacheKey, infos, TimeSpan.FromHours(1));

            return infos;
        }

        public async ValueTask<string?> FindLightIdAsync(string name)
        {
            IReadOnlyCollection<LightInfo> lights = await GetLightsAsync();

            return lights.FirstOrDefault(l
                => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                   l.Id.Equals(name, StringComparison.OrdinalIgnoreCase))?.Id;
        }
    }
}