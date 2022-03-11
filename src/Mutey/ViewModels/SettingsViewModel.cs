namespace Mutey.ViewModels
{
    using System;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors.Core;
    using Mutey.Core.Input;
    using Mutey.Core.Settings;
    using Mutey.Popup;
    using Prism.Mvvm;

    internal class SettingsViewModel : BindableBase
    {
        private readonly ISettingsStore settingsStore;
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


        public SettingsViewModel( ISettingsStore settingsStore )
        {
            this.settingsStore = settingsStore;
            
            SaveCommand = new ActionCommand( Save );

            var input = settingsStore.Get<InputSettings>();
            var view = settingsStore.Get<ViewSettings>();
            isPttEnabled = input.DefaultTransformMode.HasFlag( TransformModes.Ptt );
            isToggleEnabled = input.DefaultTransformMode.HasFlag( TransformModes.Toggle );
            smartPttActivationMilliSeconds = (int) input.SmartPttActivationDuration.TotalMilliseconds;
            popupMode = view.MuteStatePopupMode;
            popupSize = view.MuteStatePopupSize;
        }

        private void Save()
        {
            settingsStore.Set<InputSettings>( Save );
            settingsStore.Set<ViewSettings>( Save );
        }

        private ViewSettings Save( ViewSettings settings )
        {
            return settings with
            {
                MuteStatePopupMode = PopupMode,
                MuteStatePopupSize = PopupSize
            };
        }

        private InputSettings Save( InputSettings settings )
        {
            return settings with
            {
                SmartPttActivationDuration = TimeSpan.FromMilliseconds( SmartPttActivationMilliSeconds ),
                DefaultTransformMode = GetTransformModes(),
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
