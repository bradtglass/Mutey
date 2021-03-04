using Prism.Mvvm;

namespace Mutey.Popup
{
    internal class MicStatePopupViewModel : BindableBase
    {
        private bool isVisible;
        private MuteState state;

        public MuteState State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }
    }
}