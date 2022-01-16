using System.Threading.Tasks;

namespace HueMicIndicator.Hue;

public interface IInteractiveLoginHelper
{
    Task<bool> RequestButtonPressAsync();
}