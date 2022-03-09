namespace Mutey.ViewModels
{
    using System;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors.Core;
    using Mutey.Core.Settings;
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

            var current = SettingsStore.Get<MuteySettings>();
            isPttEnabled = current.DefaultTransformMode.HasFlag( TransformModes.Ptt );
            isToggleEnabled = current.DefaultTransformMode.HasFlag( TransformModes.Toggle );
            smartPttActivationMilliSeconds = (int) current.SmartPttActivationDuration.TotalMilliseconds;
            popupMode = current.MuteStatePopupMode;
            popupSize = current.MuteStatePopupSize;
        }

        private void Save()
        {
            SettingsStore.Set<MuteySettings>( Save );
        }

        private MuteySettings Save( MuteySettings settings )
        {
            return settings with
            {
                SmartPttActivationDuration = TimeSpan.FromMilliseconds( SmartPttActivationMilliSeconds ),
                DefaultTransformMode = GetTransformModes(),
                MuteStatePopupMode = PopupMode,
                MuteStatePopupSize = PopupSize
            };
        }

        private TransformModes GetTransformModes()
        {
            TransformModes modes = 0;
            if ( IsPttEnabled )
            {
                modes |= TransformModes.Ptt;
            }

            if ( IsToggleEnabled )
            {
                modes |= TransformModes.Toggle;
            }

            return modes;
        }
    }
}
