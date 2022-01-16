using System.Collections.Generic;
using System.Threading.Tasks;
using Q42.HueApi.Interfaces;

namespace HueMicIndicator.Hue;

public class HueStateWrapper : IHueState
{
    private readonly IReadOnlyCollection<IHueState> states;

    public HueStateWrapper(IReadOnlyCollection<IHueState> states)
    {
        this.states = states;
    }


    public async Task ApplyAsync(IHueClient client)
        => await Parallel.ForEachAsync(states,
            new ParallelOptions { MaxDegreeOfParallelism = 4 },
            async (state, ct) => await state.ApplyAsync(client));
}