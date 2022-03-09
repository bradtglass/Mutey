namespace Mutey.Audio.Mute
{
    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using NAudio.CoreAudioApi;
    using Nito.AsyncEx;
    using NLog;

    [ UsedImplicitly ]
    public class SystemMuteControl : ISystemMuteControl, IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly AsyncContextThread syncThread = new();
        private MMDevice? current;
        private bool disposed;

        public void Dispose()
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;
            syncThread.Context.SynchronizationContext.Send( () => current?.Dispose() );
            syncThread.Dispose();
        }

        public void Mute()
        {
            SetMuteState( MuteState.Muted );
        }

        public void Unmute()
        {
            SetMuteState( MuteState.Unmuted );
        }

        public MuteState GetState()
        {
            AssertNotDisposed();

            var state = syncThread.Context.SynchronizationContext.Send( GetStateSync );

            return state;
        }

        public event EventHandler<MuteChangedEventArgs>? StateChanged;

        private void SetMuteState( MuteState state )
        {
            AssertNotDisposed();

            syncThread.Context.SynchronizationContext.Send( () => SetMuteStateSync( state ) );
        }

        /// <remarks>
        ///     This method must only be called from the synchronised <see cref="syncThread" /> because the
        ///     <see cref="MMDevice" /> COM interface does not appear to behave when accessed from multiple threads..
        /// </remarks>
        private void SetMuteStateSync( MuteState state )
        {
            AssertSync();

            using var activeMicrophone = GetActiveMicrophoneSync();

            if ( activeMicrophone == null )
            {
                logger.Warn( "Cannot set state, active mic not found" );
                return;
            }

            bool newMuteValue = state == MuteState.Muted;

            if ( activeMicrophone.AudioEndpointVolume.Mute == newMuteValue )
            {
                logger.Trace( "Skipping setting mute state, mic is already in correct state" );
                return;
            }

            logger.Trace( "Setting mute state of active mic ({Mic}) to {State}", activeMicrophone.FriendlyName,
                          newMuteValue );
            activeMicrophone.AudioEndpointVolume.Mute = newMuteValue;

            InvokeStateChanged( state );
        }

        /// <remarks>
        ///     This method must only be called from the synchronised <see cref="syncThread" /> because the
        ///     <see cref="MMDevice" /> COM interface does not appear to behave when accessed from multiple threads..
        /// </remarks>
        private MuteState GetStateSync()
        {
            AssertSync();

            using var device = GetActiveMicrophoneSync();
            if ( device == null )
            {
                logger.Debug( "Cannot get state, no active mic" );
                return MuteState.Unknown;
            }

            if ( device.State != DeviceState.Active )
            {
                logger.Debug( "Cannot get state, the default mic is not active" );
                return MuteState.Unknown;
            }

            var muteState = device.AudioEndpointVolume.Mute ? MuteState.Muted : MuteState.Unmuted;
            logger.Trace( "Retrieved current state of {Mic}: {State}", device.FriendlyName, muteState );

            return muteState;
        }

        private void CurrentVolumeChanged( AudioVolumeNotificationData data )
        {
            var state = data.Muted ? MuteState.Muted : MuteState.Unmuted;
            logger.Trace( "Receieved a notification that the default mic state had changed, new state is {State}",
                          state );

            InvokeStateChanged( state );
        }

        private void InvokeStateChanged( MuteState state )
        {
            StateChanged?.Invoke( this, new MuteChangedEventArgs( state ) );
        }

        /// <remarks>
        ///     This method must only be called from the synchronised <see cref="syncThread" /> because the
        ///     <see cref="MMDevice" /> COM interface does not appear to behave when accessed from multiple threads..
        /// </remarks>
        private MMDevice? GetActiveMicrophoneSync()
        {
            AssertSync();

            EnsureCurrentIsSetSync();

            if ( current == null )
            {
                return null;
            }

            using MMDeviceEnumerator enumerator = new();
            return enumerator.GetDevice( current.ID );
        }

        private void AssertNotDisposed()
        {
            if ( disposed )
            {
                throw new ObjectDisposedException( nameof( SystemMuteControl ) );
            }
        }

        private void AssertSync()
        {
            AssertNotDisposed();

            if ( SynchronizationContext.Current != syncThread.Context.SynchronizationContext )
            {
                throw new SynchronizationLockException();
            }
        }

        /// <remarks>
        ///     This method must only be called from the synchronised <see cref="syncThread" /> because the
        ///     <see cref="MMDevice" /> COM interface does not appear to behave when accessed from multiple threads..
        /// </remarks>
        private void EnsureCurrentIsSetSync()
        {
            AssertSync();

            using MMDeviceEnumerator mmDeviceEnumerator = new();

            var defaultMic = mmDeviceEnumerator.GetDefaultAudioEndpoint( DataFlow.Capture, Role.Communications );

            var suppressDispose = false;
            try
            {
                if ( current == null )
                {
                    current = defaultMic;
                    defaultMic.AudioEndpointVolume.OnVolumeNotification += CurrentVolumeChanged;
                    suppressDispose = true;
                }
                else
                {
                    if ( current.ID != defaultMic.ID )
                    {
                        current.AudioEndpointVolume.OnVolumeNotification -= CurrentVolumeChanged;
                        current.Dispose();

                        current = defaultMic;
                        defaultMic.AudioEndpointVolume.OnVolumeNotification += CurrentVolumeChanged;
                        suppressDispose = true;
                    }
                }
            }
            finally
            {
                if ( !suppressDispose )
                {
                    defaultMic.Dispose();
                }
            }
        }
    }
}
