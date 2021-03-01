using System;
using JetBrains.Annotations;
using NAudio.CoreAudioApi;
using NLog;

namespace Mutey.Mute
{
    [UsedImplicitly]
    public class SystemMuteControl : ISystemMuteControl, IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        private MMDevice? current;

        public void Dispose()
        {
            current?.Dispose();
        }

        public void Mute()
            => SetMuteState(MuteState.Muted);

        public void Unmute()
            => SetMuteState(MuteState.Unmuted);

        private void SetMuteState(MuteState state)
        {
            using MMDevice? activeMicrophone = GetActiveMicrophone();

            if (activeMicrophone == null)
            {
                logger.Warn("Cannot set state, active mic not found");
                return;
            }

            bool newMuteValue = state == MuteState.Muted;

            if (activeMicrophone.AudioEndpointVolume.Mute == newMuteValue)
            {
                logger.Trace("Skipping setting mute state, mic is already in correct state");
                return;
            }
            
            logger.Trace("Setting mute state of active mic ({Mic}) to {State}", activeMicrophone.FriendlyName, newMuteValue);
            activeMicrophone.AudioEndpointVolume.Mute = newMuteValue;

            InvokeStateChanged(state);
        }

        public MuteState GetState()
        {
            using MMDevice? device = GetActiveMicrophone();
            if (device == null)
            {
                logger.Debug("Cannot get state, no active mic");
                return MuteState.Unknown;
            }

            if (device.State != DeviceState.Active)
            {
                logger.Debug("Cannot get state, the default mic is not active");
                return MuteState.Unknown;
            }

            MuteState muteState = device.AudioEndpointVolume.Mute ? MuteState.Muted : MuteState.Unmuted;
            logger.Trace("Retrieved current state of {Mic}: {State}", device.FriendlyName, muteState);
            
            return muteState;
        }

        public event EventHandler<MuteChangedEventArgs>? StateChanged;

        private void CurrentVolumeChanged(AudioVolumeNotificationData data)
        {
            MuteState state = data.Muted ? MuteState.Muted : MuteState.Unmuted;
            logger.Trace("Receieved a notification that the default mic state had changed, new state is {State}", state);

            InvokeStateChanged(state);
        }

        private void InvokeStateChanged(MuteState state)
            => StateChanged?.Invoke(this, new MuteChangedEventArgs(state));

        private MMDevice? GetActiveMicrophone()
        {
            EnsureCurrentIsSet();

            if (current == null)
                return null;

            using MMDeviceEnumerator enumerator = new();
            return enumerator.GetDevice(current.ID);
        }

        private void EnsureCurrentIsSet()
        {
            using MMDeviceEnumerator mmDeviceEnumerator = new();

            MMDevice defaultMic = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

            bool suppressDispose = false;
            try
            {
                if (current == null)
                {
                    current = defaultMic;
                    defaultMic.AudioEndpointVolume.OnVolumeNotification += CurrentVolumeChanged;
                    suppressDispose = true;
                }
                else
                {
                    if (current.ID != defaultMic.ID)
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
                if (!suppressDispose)
                    defaultMic.Dispose();
            }
        }
    }
}