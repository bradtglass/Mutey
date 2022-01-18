using System.Collections.Immutable;
using System.Threading.Tasks;
using HueMicIndicator.Core.Settings;

namespace HueMicIndicator.Hue.State;

public class HueStateStore
{
    private readonly HueContext context;

    internal HueStateStore(HueContext context)
    {
        this.context = context;
    }

    public HueStateSetting Get(bool isActive)
    {
        var key = GetStateKey(isActive);
        ImmutableDictionary<string, HueStateSetting> settings = SettingsStore.Get<HueSettings>(HueSettings.Sub).States;
        if (settings.TryGetValue(key, out var setting))
            return setting;

        return HueStateSetting.Empty;
    }

    public async ValueTask<IHueState> GetStateAsync(bool isActive)
    {
        var setting = Get(isActive);

        return await HueState.CreateAsync(setting, context);
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