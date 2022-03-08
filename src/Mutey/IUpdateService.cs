namespace Mutey
{
    using System;
    using System.Threading.Tasks;

    public interface IUpdateService
    {
        UpdateState State { get; }
        event EventHandler? StateChanged;
        Task<UpdateState> RefreshStateAsync();
        Task UpdateAsync();
    }
}
