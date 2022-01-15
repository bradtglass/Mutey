using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace HueMicIndicator.Hue
{
    public class HueState : IHueState
    {
        private readonly (double, double)? colour;
        private readonly string[] lights;
        private readonly bool? on;

        public HueState(bool? on, (double, double)? colour, params string[] lights)
        {
            this.on = on;
            this.colour = colour;
            this.lights = lights;
        }

        public async Task ApplyAsync(IHueClient client)
        {
            var command = new LightCommand();

            if (colour.HasValue)
                command.SetColor(colour.Value.Item1, colour.Value.Item2);

            command.On = on;

            await client.SendCommandAsync(command, lights);
        }
    }
}