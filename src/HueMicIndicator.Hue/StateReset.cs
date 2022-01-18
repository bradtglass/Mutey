using System;
using System.Threading.Tasks;
using HueMicIndicator.Hue.State;

namespace HueMicIndicator.Hue;

public sealed class StateReset : IAsyncDisposable
{
    private readonly HueContext context;
    private readonly IHueState state;

    public StateReset(IHueState state, HueContext context)
    {
        this.state = state;
        this.context = context;
    }

    public async ValueTask DisposeAsync()
        => await ResetAsync();

    public async ValueTask ResetAsync()
        => await state.ApplyAsync(context);
}