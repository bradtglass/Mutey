using System;
using NAudio.CoreAudioApi;

namespace Mutey.Mute
{
    public class SystemMuteControl : ISystemMuteControl, IDisposable
    {
        private MMDevice? current;

        public void Dispose()
        {
            current?.Dispose();
        }

        public void Mute()
        {
            using   MMDevice? activeMicrophone = GetActiveMicrophone();

            if (activeMicrophone == null)
                return;

            activeMicrophone.AudioEndpointVolume.Mute = true;

           InvokeStateChanged(MuteState.Muted);
        }

        public void Unmute()
        {
           using MMDevice? activeMicrophone = GetActiveMicrophone();

            if (activeMicrophone == null)
                return;

            activeMicrophone.AudioEndpointVolume.Mute = false;

           InvokeStateChanged(MuteState.Unmuted);
        }

        public MuteState GetState()
        {
            using MMDevice? device = GetActiveMicrophone();
            if (device == null || device.State != DeviceState.Active)
                return MuteState.Unknown;

            return device.AudioEndpointVolume.Mute ? MuteState.Muted : MuteState.Unmuted;
        }

        public event EventHandler<MuteChangedEventArgs>? StateChanged;

        private void CurrentVolumeChanged(AudioVolumeNotificationData data)
        {
            MuteState state = data.Muted ? MuteState.Muted : MuteState.Unmuted;

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
                if(!suppressDispose)
                    defaultMic.Dispose();
            }
        }
    }
}