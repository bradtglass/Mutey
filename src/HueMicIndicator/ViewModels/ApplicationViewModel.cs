using System;

namespace HueMicIndicator.ViewModels
{
    public class ApplicationViewModel : IDisposable
    {
        public StateViewModel State { get; } = new();

        public void Dispose()
        {
            State.Dispose();
        }
    }
}