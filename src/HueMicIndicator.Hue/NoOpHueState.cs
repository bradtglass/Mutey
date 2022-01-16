using System.Threading.Tasks;
using Q42.HueApi.Interfaces;

namespace HueMicIndicator.Hue;

public class NoOpHueState : IHueState
{
    private NoOpHueState() { }
    public static NoOpHueState Instance { get; } = new();

    public Task ApplyAsync(IHueClient _)
        => Task.CompletedTask;
}