using System.Collections.Generic;
using System.Threading.Tasks;

namespace HueMicIndicator.Hue.State;

public class HueStateWrapper : IHueState
{
    private readonly IReadOnlyCollection<IHueState> states;

    public HueStateWrapper(IReadOnlyCollection<IHueState> states)
    {
        this.states = states;
    }

    public async Task ApplyAsync(HueContext context)
        => await Parallel.ForEachAsync(states,
            new ParallelOptions { MaxDegreeOfParallelism = 4 },
            async (state, _) => await state.ApplyAsync(context));
}