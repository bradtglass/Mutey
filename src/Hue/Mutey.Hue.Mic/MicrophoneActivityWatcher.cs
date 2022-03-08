using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Mutey.Hue.Mic
{
    public class MicrophoneActivityWatcher : IDisposable
    {
        private static readonly uint processId = (uint)Process.GetCurrentProcess().Id;
        private readonly CancellationTokenSource cts = new();

        private MicrophoneActivityWatcher() { }

        public bool IsActive { get; private set; }

        public void Dispose()
        {
            cts.Dispose();
        }

        public event EventHandler<MicrophoneActivityEventArgs>? Notify;

        public static MicrophoneActivityWatcher Create()
        {
            MicrophoneActivityWatcher watcher = new();

            Task.Factory.StartNew(watcher.WatchLoop, watcher.cts.Token);

            return watcher;
        }

        private static bool GetIsActive()
        {
            using MMDeviceEnumerator mmDeviceEnumerator = new();
            using var defaultMic = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            var sessionManager = defaultMic.AudioSessionManager;

            return sessionManager.Sessions.Enumerate()
                .Where(s => !s.IsSystemSoundsSession)
                .Where(s => s.GetProcessID != processId)
                .Any(s => s.State == AudioSessionState.AudioSessionStateActive);
        }

        private async Task WatchLoop()
        {
            while (true)
            {
                try
                {
                    var isNowActive = GetIsActive();
                    if (isNowActive != IsActive)
                    {
                        IsActive = isNowActive;
                        Notify?.Invoke(this, new MicrophoneActivityEventArgs(isNowActive));
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(1000, cts.Token);
            }
        }
    }
}