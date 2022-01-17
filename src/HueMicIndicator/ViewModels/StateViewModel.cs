using System;
using System.Windows.Threading;
using HueMicIndicator.Hue;
using HueMicIndicator.Mic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Nito.AsyncEx;

namespace HueMicIndicator.ViewModels
{
    public sealed class StateViewModel : ObservableObject, IDisposable
    {
        private readonly AsyncLock changeLock = new();
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private readonly HueContext context;
        private readonly MicrophoneActivityWatcher microphoneWatcher;

        private bool isActive;

        public StateViewModel(HueContext context)
        {
            this.context = context;
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

        private async void OnStateChange(object? sender, MicrophoneActivityEventArgs e)
        {
            using var _ = await changeLock.LockAsync();

            try
            {
                await dispatcher.InvokeAsync(() => IsActive = e.IsActive);
                await context.SetStateAsync(e.IsActive);
            }
            catch (Exception exception)
            {
                // TODO Log exception properly
                Console.WriteLine(exception);
            }
        }
    }
}