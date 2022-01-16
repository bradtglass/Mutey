using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using HueMicIndicator.Core.Settings;

namespace HueMicIndicator.Hue;

public class HueStateStore
{
    private readonly HueHandler handler;

    internal HueStateStore(HueHandler handler)
    {
        this.handler = handler;
    }

    public async ValueTask<IHueState> GetAsync(bool isActive)
    {
        var key = GetStateKey(isActive);
        ImmutableDictionary<string, HueStateSetting> settings = SettingsStore.Get<HueSettings>(HueSettings.Sub).States;
        if (settings.TryGetValue(key, out var setting))
            return await BuildStateAsync(setting);

        return NoOpHueState.Instance;
    }

    private async ValueTask<IHueState> BuildStateAsync(HueStateSetting setting)
    {
        List<IHueState> states = new();
        foreach (IGrouping<HueLightSetting, KeyValuePair<string, HueLightSetting>> lightGroup in setting.Lights
                     .GroupBy(l => l.Value))
        {
            var state = await BuildStateForLightGroupAsync(lightGroup.Key, lightGroup.Select(kvp => kvp.Key));

            if (state != null)
                states.Add(state);
        }

        return states.Count switch
        {
            0 => NoOpHueState.Instance,
            1 => states[0],
            _ => new HueStateWrapper(states)
        };
    }

    private async ValueTask<IHueState?> BuildStateForLightGroupAsync(HueLightSetting setting,
        IEnumerable<string> lights)
    {
        List<string> ids = new();

        foreach (var light in lights)
            if (await handler.FindLightIdAsync(light) is { } id)
                ids.Add(id);

        if (ids.Count == 0)
            return null;

        return new HueLightState(setting, ids);
    }

    public void Set(bool isActive, HueStateSetting setting)
    {
        var key = GetStateKey(isActive);
        SettingsStore.Set<HueSettings>(HueSettings.Sub, s => s with
        {
            States = s.States.SetItem(key, setting)
        });
    }

    private static string GetStateKey(bool isActive)
        => isActive ? "Active" : "Inactive";
}