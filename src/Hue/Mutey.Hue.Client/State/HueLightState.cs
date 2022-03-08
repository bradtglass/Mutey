using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi;

namespace Mutey.Hue.Client.State;

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
        if (setting.ResetFirst)
            await ApplyWithResetAsync(context);
        else
            await ApplyWithoutResetAsync(context);
    }


    public IEnumerable<string> GetAffectedLights()
        => lights;

    private async ValueTask ApplyWithoutResetAsync(HueContext context)
        => await ApplyCoreAsync(context, lights, setting);

    private static async ValueTask ApplyCoreAsync(HueContext context, IEnumerable<string> lights,
        params HueLightSetting?[] settings)
    {
        var command = new LightCommand();
        foreach (var setting in settings) 
            setting?.Apply(command);

        await context.SendCommandAsync(command, lights);
    }

    private async ValueTask ApplyWithResetAsync(HueContext context)
    {
        foreach (var light in lights)
        {
            var lastState = context.GetLastLightState(light);
            await ApplyCoreAsync(context, new[] { light }, lastState, setting);
        }
    }
}