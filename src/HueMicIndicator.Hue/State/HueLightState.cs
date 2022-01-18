using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi;

namespace HueMicIndicator.Hue.State;

internal class HueLightState : IHueState
{
    private readonly IReadOnlyCollection<string> lights;
    private readonly HueLightSetting setting;

    public HueLightState(HueLightSetting setting, IReadOnlyCollection<string> lights)
    {
        this.setting = setting;
        this.lights = lights;
    }

    public async Task ApplyAsync(HueContext context)
    {
        var command = new LightCommand();
        setting.Apply(command);

        await context.SendCommandAsync(command, lights);
    }

    public IEnumerable<string> GetAffectedLights()
        => lights;
}