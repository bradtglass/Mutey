namespace Mutey.Hue.Client.State
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHueState
    {
        Task ApplyAsync( HueContext context );

        IEnumerable<string> GetAffectedLights();
    }
}
