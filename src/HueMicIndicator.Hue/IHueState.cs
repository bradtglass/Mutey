using System.Threading.Tasks;
using Q42.HueApi.Interfaces;

namespace HueMicIndicator.Hue
{
    public interface IHueState
    {
        Task ApplyAsync(IHueClient client);
    }
}