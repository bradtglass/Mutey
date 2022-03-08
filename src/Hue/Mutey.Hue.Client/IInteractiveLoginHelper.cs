using System.Threading.Tasks;

namespace Mutey.Hue.Client;

public interface IInteractiveLoginHelper
{
    Task<bool> RequestButtonPressAsync();
}