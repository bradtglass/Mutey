using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using NLog;

namespace Mutey.Output
{
    public abstract class UiAutomationCall : ICall
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IntPtr window;

        protected UiAutomationCall(IntPtr window)
        {
            if (window == IntPtr.Zero)
                throw new ArgumentException("Window cannot be default", nameof(window));

            this.window = window;
        }

        public abstract string Name { get; }
        public abstract bool CanToggle { get; }
        public abstract bool CanMuteUnmute { get; }
        public abstract bool CanRaiseMuteStateChanged { get; }
        public abstract void Toggle();
        public abstract void Mute();
        public abstract void Unmute();
        public abstract MuteState GetState();
        public abstract event EventHandler? MuteStateChanged;
        public event EventHandler? Ended;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);


        protected static void CommitAutomationInput(Action inputCallback)
        {
            IntPtr activeWindow = GetForegroundWindow();
            try
            {
                inputCallback();
            }
            finally
            {
                if (activeWindow == IntPtr.Zero)
                    logger.Warn("Cannot reset active window, pointer is zero");
                else
                    try
                    {
                        SetForegroundWindow(activeWindow);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, "Error switching back to original active window");
                    }
            }
        }

        protected AutomationElement GetWindowElement()
            => AutomationElement.FromHandle(window);

        protected void End()
            => Ended?.Invoke(this, EventArgs.Empty);
    }
}