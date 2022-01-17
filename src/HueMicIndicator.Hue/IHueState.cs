using System.Threading.Tasks;

namespace HueMicIndicator.Hue
{
    public interface IHueState
    {
        Task ApplyAsync(HueContext context);
    }
}