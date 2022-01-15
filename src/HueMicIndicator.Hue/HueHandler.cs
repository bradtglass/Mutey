using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace HueMicIndicator.Hue
{
    public class HueHandler
    {
        private const string appKey = "";

        private readonly HueStateStore stateStore = new();
        private IHueClient? client;

        public async Task<IHueClient> LoginAsync()
        {
            // TODO Login interactive
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(30));
            var bridge = bridges.First();

            var innerClient = new LocalHueClient(bridge.IpAddress, appKey);
            client = innerClient;

            return innerClient;
        }

        public async Task ChangeState(bool isActive)
        {
            if (client is not { } innerClient)
                innerClient = await LoginAsync();

            var state = stateStore.GetState(isActive);
            await state.ApplyAsync(innerClient);
        }
    }
}