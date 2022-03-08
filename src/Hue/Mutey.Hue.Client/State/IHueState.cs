using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mutey.Hue.Client.State;

public interface IHueState
{
    Task ApplyAsync(HueContext context);

    IEnumerable<string> GetAffectedLights();
}