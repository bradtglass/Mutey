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

            foreach (int key in keys)
                PostMessage(process.MainWindowHandle, WM_KEYDOWN, key, 0);

            foreach (int key in keys)
                PostMessage(process.MainWindowHandle, WM_KEYUP, key, 0);
        }

        public void Mute()
            => throw new NotSupportedException();

        public void Unmute()
            => throw new NotSupportedException();

        public event EventHandler? Ended;

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        protected void End()
            => Ended?.Invoke(this, EventArgs.Empty);

        // ReSharper disable InconsistentNaming
        private const uint WM_KEYDOWN = 0x0100;

        private const uint WM_KEYUP = 0x0101;
        // ReSharper restore InconsistentNaming
    }
}