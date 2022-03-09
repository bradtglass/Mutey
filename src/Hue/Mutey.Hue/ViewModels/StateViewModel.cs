namespace Mutey.Hue.ViewModels
{
    using System;
    using System.Windows.Threading;
    using Microsoft.Toolkit.Mvvm.ComponentModel;
    using Mutey.Hue.Client;
    using Mutey.Hue.Mic;
    using Nito.AsyncEx;

    public sealed class StateViewModel : ObservableObject, IDisposable
    {
        private readonly AsyncLock changeLock = new();
        private readonly HueContext context;
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private readonly MicrophoneActivityWatcher microphoneWatcher;

        private bool isActive;

        private bool setActiveOverride;

        public bool IsActive
        {
            get => isActive;
            private set
            {
                if ( SetProperty( ref isActive, value ) )
                {
                    PropagateState( value );
                }
            }
        }

        public bool SetActiveOverride
        {
            get => setActiveOverride;
            set
            {
                if ( SetProperty( ref setActiveOverride, value ) ) RefreshState();
            }
        }

        public StateViewModel( HueContext context )
        {
            this.context = context;
            microphoneWatcher = MicrophoneActivityWatcher.Create();
            microphoneWatcher.Notify += OnStateChange;

            RefreshState();
        }

        public void Dispose()
        {
            microphoneWatcher.Dispose();
        }

        private void OnStateChange( object? sender, MicrophoneActivityEventArgs e )
        {
            RefreshState();
        }

        private void RefreshState()
        {
            dispatcher.InvokeAsync( () => IsActive = SetActiveOverride || microphoneWatcher.IsActive );
        }

        private async void PropagateState( bool value )
        {
            using var _ = await changeLock.LockAsync();

            try
            {
                await context.SetStateAsync( value );
            }
            catch ( Exception exception )
            {
                // TODO Log exception properly
                Console.WriteLine( exception );
            }
        }
    }
}
