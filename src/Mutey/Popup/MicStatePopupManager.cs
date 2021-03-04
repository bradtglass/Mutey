using System;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Mutey.Popup
{
    [UsedImplicitly]
    public class MicStatePopupManager : IDisposable
    {
        private readonly MicStatePopupViewModel controller;
        private readonly object currentLifetimeLock = new();
        private readonly MicStatePopup popup;

        private Lifetime? currentLifetime;

        public MicStatePopupManager()
        {
            controller = new MicStatePopupViewModel();
            popup = new MicStatePopup(controller);

            popup.Show();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public Lifetime Show(MuteState initialState)
        {
            Lifetime lifetime = BeginLifetime();
            lifetime.ChangeState(initialState);
            lifetime.Show();

            return lifetime;
        }

        public void Flash(MuteState state)
        {
            Lifetime lifetime = Show(state);

            DispatcherTimer timer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(3),
                Tag = lifetime
            };
            timer.Tick += EndFlashTick;
            timer.Start();
        }

        private static void EndFlashTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer) sender;
            timer.Stop();
            timer.Tick -= EndFlashTick;

            Lifetime lifetime = (Lifetime) timer.Tag;
            lifetime.Hide();
        }

        private bool ShowPopup(Lifetime lifetime)
            => CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.IsVisible = true);

        private bool HidePopup(Lifetime lifetime)
            => CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.IsVisible = false);

        private bool ChangeIcon(Lifetime lifetime, MuteState muteState)
            => CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.State = muteState);

        private bool CheckLifetimeAndRunUiCallback(Lifetime lifetime, Action<MicStatePopupManager> callback)
        {
            lock (currentLifetimeLock)
            {
                if (!ReferenceEquals(currentLifetime, lifetime))
                    return false;

                if (Application.Current.Dispatcher.CheckAccess())
                    callback(this);
                else
                    Application.Current.Dispatcher.Invoke(callback, this);

                return true;
            }
        }

        private Lifetime BeginLifetime()
        {
            lock (currentLifetimeLock)
            {
                currentLifetime = new Lifetime(this);
                return currentLifetime;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            popup.Close();
        }

        ~MicStatePopupManager()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        ///     Represents the lifetime of a popup which, when disposed, transitions the popup to a hidden state.
        /// </summary>
        public sealed class Lifetime
        {
            private readonly MicStatePopupManager manager;

            public Lifetime(MicStatePopupManager manager)
            {
                this.manager = manager;
            }

            public bool ChangeState(MuteState state)
                => manager.ChangeIcon(this, state);

            public bool Show()
                => manager.ShowPopup(this);

            public bool Hide()
                => manager.HidePopup(this);
        }
    }
}