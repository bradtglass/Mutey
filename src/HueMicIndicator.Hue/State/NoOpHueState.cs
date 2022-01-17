using System.Threading.Tasks;

namespace HueMicIndicator.Hue.State;

public class NoOpHueState : IHueState
{
    private NoOpHueState() { }
    public static NoOpHueState Instance { get; } = new();

    public Task ApplyAsync(HueContext _)
        => Task.CompletedTask;
}