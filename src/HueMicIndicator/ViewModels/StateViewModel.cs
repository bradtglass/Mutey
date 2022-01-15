using System;
using System.Windows.Threading;
using HueMicIndicator.Mic;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HueMicIndicator.ViewModels
{
    public class StateViewModel : ObservableObject, IDisposable
    {
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private readonly MicrophoneActivityWatcher microphoneWatcher;

        private bool isActive;

        public StateViewModel()
        {
            microphoneWatcher = MicrophoneActivityWatcher.Create();
            microphoneWatcher.Notify += OnStateChange;
        }

        public bool IsActive
        {
            get => isActive;
            private set => SetProperty(ref isActive, value);
        }

        public void Dispose()
        {
            microphoneWatcher.Dispose();
        }

        private void OnStateChange(object sender, MicrophoneActivityEventArgs e)
            => dispatcher.InvokeAsync(() => IsActive = e.IsActive);
    }
}