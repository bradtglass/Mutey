using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HueMicIndicator.Hue.State;

internal class HueStateWrapper : IHueState
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

    public IEnumerable<string> GetAffectedLights()
        => states.SelectMany(s => s.GetAffectedLights());
}