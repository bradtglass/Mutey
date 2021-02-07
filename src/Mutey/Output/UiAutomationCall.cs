using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Automation;
using NLog;

namespace Mutey.Output
{
    public abstract class UiAutomationCall : ICall
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll", SetLastError=true)]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        
        private readonly IntPtr window;
        private readonly Task<AutomationElement> cachedMuteButtonElement;

        protected UiAutomationCall(string name, IntPtr window)
        {
            if (window == IntPtr.Zero)
                throw new ArgumentException("Window cannot be default", nameof(window));

            cachedMuteButtonElement = Task.Run(GetMuteButton);
            
            this.window = window;
            Name = name;
        }

        public string Name { get; }
        public bool CanToggle { get; } = true;
        public virtual bool CanMuteUnmute { get; } = false;

        public void Toggle()
        {
            AutomationElement muteButtonElement = cachedMuteButtonElement.GetAwaiter().GetResult();
            IntPtr activeWindow = GetForegroundWindow();
            try
            {
                ToggleInternal(muteButtonElement);
            }
            finally
            {
                if (activeWindow == IntPtr.Zero)
                {
                    logger.Warn("Cannot reset active window, pointer is zero");
                }
                else
                {
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
        }

        private AutomationElement GetMuteButton()
        {
            AutomationElement windowElement = GetWindowElement();

            CacheRequest cacheRequest = new();
            cacheRequest.Add(TogglePattern.Pattern);
            cacheRequest.Add(TogglePattern.ToggleStateProperty);

            AutomationElement muteButtonElement;
            using (cacheRequest.Activate())
            {
                muteButtonElement = GetMuteButton(windowElement);
            }

            return muteButtonElement;
        }

        private AutomationElement GetWindowElement()
            => AutomationElement.FromHandle(window);

        protected abstract void ToggleInternal(AutomationElement muteButton);

        protected abstract AutomationElement GetMuteButton(AutomationElement element);

        public virtual void Mute()
            => throw new NotSupportedException();

        public virtual void Unmute()
            => throw new NotSupportedException();

        public event EventHandler? Ended;

        protected void End()
            => Ended?.Invoke(this, EventArgs.Empty);
    }
}