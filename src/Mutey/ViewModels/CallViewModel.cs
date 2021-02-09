using System.Threading;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Output;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class CallViewModel : BindableBase
    {
        private readonly ICall call;

        private MuteState muteState;

        public CallViewModel(ICall call)
        {
            this.call = call;

            if (call.CanRaiseMuteStateChanged)
                call.MuteStateChanged += (_, _) => UpdateMuteState();

            ToggleCommand = new ActionCommand(Toggle);
        }

        public MuteState MuteState
        {
            get => muteState;
            set => SetProperty(ref muteState, value);
        }

        public ICommand ToggleCommand { get; }

        private void Toggle()
        {
            call.Toggle();

            if (call.CanRaiseMuteStateChanged) 
                return;
            
            Thread.Sleep(20);
            UpdateMuteState();
        }

        private void UpdateMuteState()
            => MuteState = call.GetState();
    }
}