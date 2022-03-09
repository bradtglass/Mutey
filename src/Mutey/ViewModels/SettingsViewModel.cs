namespace Mutey.ViewModels
{
    using System;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors.Core;
    using Mutey.Popup;
    using Prism.Mvvm;

    internal class SettingsViewModel : BindableBase
    {
        private bool isPttEnabled;
        private bool isToggleEnabled;
        private PopupMode popupMode;
        private int popupSize;
        private int smartPttActivationMilliSeconds;

        public int PopupSize
        {
            get => popupSize;
            set => SetProperty( ref popupSize, value );
        }

        public PopupMode PopupMode
        {
            get => popupMode;
            set => SetProperty( ref popupMode, value );
        }

        public bool IsPttEnabled
        {
            get => isPttEnabled;
            set => SetProperty( ref isPttEnabled, value );
        }

        public bool IsToggleEnabled
        {
            get => isToggleEnabled;
            set => SetProperty( ref isToggleEnabled, value );
        }

        public int SmartPttActivationMilliSeconds
        {
            get => smartPttActivationMilliSeconds;
            set => SetProperty( ref smartPttActivationMilliSeconds, value );
        }

        public ICommand SaveCommand { get; }


        public SettingsViewModel()
        {
            SaveCommand = new ActionCommand( Save );

            isPttEnabled = Settings.Default.DefaultTransformMode.HasFlag( TransformModes.Ptt );
            isToggleEnabled = Settings.Default.DefaultTransformMode.HasFlag( TransformModes.Toggle );
            smartPttActivationMilliSeconds = (int) Settings.Default.SmartPttActivationDuration.TotalMilliseconds;
            popupMode = Settings.Default.MuteStatePopupMode;
            popupSize = Settings.Default.MuteStatePopupSize;
        }

        private void Save()
        {
            Settings.Default.SmartPttActivationDuration = TimeSpan.FromMilliseconds( SmartPttActivationMilliSeconds );

            TransformModes modes = 0;
            if ( IsPttEnabled )
            {
                modes |= TransformModes.Ptt;
            }

            if ( IsToggleEnabled )
            {
                modes |= TransformModes.Toggle;
            }

            Settings.Default.DefaultTransformMode = modes;
            Settings.Default.MuteStatePopupMode = PopupMode;
            Settings.Default.MuteStatePopupSize = PopupSize;

            Settings.Default.Save();
        }
    }
}
