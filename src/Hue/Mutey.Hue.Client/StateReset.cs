using System;
using System.Threading.Tasks;
using Mutey.Hue.Client.State;

namespace Mutey.Hue.Client;

using Mutey.Hue.Client.State;

public sealed class StateReset : IAsyncDisposable
{
    private readonly HueContext context;
    private readonly bool cacheOnReset;
    private readonly IHueState state;

    public StateReset(IHueState state, HueContext context, bool cacheOnReset)
    {
        this.state = state;
        this.context = context;
        this.cacheOnReset = cacheOnReset;
    }

    public async ValueTask DisposeAsync()
        => await ResetAsync();

    public async ValueTask ResetAsync()
        => await context.ApplyStateAsync(state, cacheOnReset);
}