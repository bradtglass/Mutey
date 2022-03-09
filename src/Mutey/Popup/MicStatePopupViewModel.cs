namespace Mutey.Popup
{
    using System.Windows.Input;
    using Mutey.Audio;
    using Prism.Mvvm;

    internal class MicStatePopupViewModel : BindableBase
    {
        private bool isVisible;
        private MuteState state;

        public MuteState State
        {
            get => state;
            set => SetProperty( ref state, value );
        }

        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty( ref isVisible, value );
        }

        public ICommand PopupPressedCommand { get; }

        public MicStatePopupViewModel( ICommand popupPressedCommand )
        {
            PopupPressedCommand = popupPressedCommand;
        }
    }
}
