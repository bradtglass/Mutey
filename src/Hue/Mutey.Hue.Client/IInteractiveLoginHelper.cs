namespace Mutey.Hue.Client
{
    using System.Threading.Tasks;

    public interface IInteractiveLoginHelper
    {
        Task<bool> RequestButtonPressAsync();
    }
}
