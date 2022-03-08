﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors.Core;
using NLog;

namespace Mutey.Popup
{
    [UsedImplicitly]
    public class MicStatePopupManager : IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        private readonly MicStatePopupViewModel controller;
        private readonly object currentLifetimeLock = new();
        private readonly MicStatePopup popup;
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;

        private Lifetime? currentLifetime;

        public MicStatePopupManager()
        {
            Settings.Default.PropertyChanged += SettingsChanged;

            logger.Info("Creating new popup window");
            
            controller = new MicStatePopupViewModel(new ActionCommand(OnPopupPressed));
            popup = new MicStatePopup(controller);

            popup.Show();

            switch (Settings.Default.MuteStatePopupMode)
            {
                case PopupMode.Temporary:
                    break;
                case PopupMode.Permanent:
                    controller.IsVisible = true;
                    break;
                case PopupMode.Off:
                    controller.IsVisible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnPopupPressed()
            => PopupPressed?.Invoke(this, EventArgs.Empty);

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(Settings.MuteStatePopupMode))
                return;

            logger.Debug("Popup mode setting updated, changing popup visibility");
            // Lock to prevent any actions occurring on the popup until we've finished changing the state
            lock (currentLifetimeLock)
            {
                switch (Settings.Default.MuteStatePopupMode)
                {
                    /*Technically we should probably keep the requested state separately from the actual state so we
                     can restore it when going back to temporary mode but this seems like overkill for this 
                     specific case currently, given how often the setting is likely to change (not very).*/
                    case PopupMode.Temporary:
                    case PopupMode.Off:
                        dispatcher.Invoke(() => controller.IsVisible = false);
                        break;
                    case PopupMode.Permanent:
                        dispatcher.Invoke(() => controller.IsVisible = true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

            logger.Debug("Flash has completed, attempting to hide popup");
            Lifetime lifetime = (Lifetime) timer.Tag;
            lifetime.Hide();
        }

        private bool ShowPopup(Lifetime lifetime)
        {
            if (Settings.Default.MuteStatePopupMode != PopupMode.Temporary)
            {
                logger.Debug("Skipping hiding mute state popup");
                return true;
            }
            
            logger.Debug("Showing mute state popup");
            return CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.IsVisible = true);
        }

        private bool HidePopup(Lifetime lifetime)
        {
            if (Settings.Default.MuteStatePopupMode != PopupMode.Temporary)
            {
                logger.Debug("Skipping hiding mute state popup");
                return true;
            }

            logger.Debug("Hiding mute state popup");
            return CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.IsVisible = false);
        }

        private bool ChangeIcon(Lifetime lifetime, MuteState muteState)
            => CheckLifetimeAndRunUiCallback(lifetime, m => m.controller.State = muteState);

        private bool CheckLifetimeAndRunUiCallback(Lifetime lifetime, Action<MicStatePopupManager> callback)
        {
            lock (currentLifetimeLock)
            {
                if (!ReferenceEquals(currentLifetime, lifetime))
                {
                    logger.Trace("Lifetime has expired, skipping UI callback");
                    return false;
                }

                logger.Trace("Lifetime is valid, invoking callback");
                if (dispatcher.CheckAccess())
                    callback(this);
                else
                    dispatcher.Invoke(callback, this);

                return true;
            }
        }

        public Lifetime BeginLifetime()
        {
            lock (currentLifetimeLock)
            {
                logger.Trace("Beginning a new popup lifetime");
                currentLifetime = new Lifetime(this);
                return currentLifetime;
            }
        }

        public event EventHandler? PopupPressed;

        private void ReleaseUnmanagedResources()
        {
            try
            {
                dispatcher.Invoke(() => popup.Close());
            }
            catch (Exception e)
            {
                logger.Error(e, "Exception raised while closing popup window");
            }
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