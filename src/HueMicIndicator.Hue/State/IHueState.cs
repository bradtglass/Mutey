using System.Collections.Generic;
using System.Threading.Tasks;

namespace HueMicIndicator.Hue.State;

public interface IHueState
{
    Task ApplyAsync(HueContext context);

    IEnumerable<string> GetAffectedLights();
}