using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mutey.Output
{
    public class SendKeysCall : ICall
    {
        // ReSharper disable InconsistentNaming
        public const int LEFT_ALT = 0xA4;
        public const int LEFT_CTRL = 0xA2;
        public const int M = 0x4D;
        // ReSharper restore InconsistentNaming
        
        private readonly int[] keys;

        private readonly Process process;

        public SendKeysCall(string name, Process process, params int[] keys)
        {
            Name = name;
            this.process = process;
            this.keys = keys;
        }

        public string Name { get; }
        public bool CanToggle { get; } = true;
        public virtual bool CanMuteUnmute { get; } = false;

        public void Toggle()
        {
            if (process.MainWindowHandle == IntPtr.Zero)
                return;

            // SetForegroundWindow(process.MainWindowHandle);
            // SendInput
        }

        public void Mute()
            => throw new NotSupportedException();

        public void Unmute()
            => throw new NotSupportedException();

        public event EventHandler? Ended;

        protected void End()
            => Ended?.Invoke(this, EventArgs.Empty);
    }
}