using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Input;
using Mutey.Mute;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class MuteyViewModel :BindableBase, IMutey
    {
        private readonly IMuteHardwareManager hardwareManager;
        private readonly ISystemMuteControl systemMuteControl;

        public void RegisterHardware(IMuteHardwareDetector detector)
        {
            hardwareManager.RegisterHardwareDetector(detector);
        }

        public MuteyViewModel(IMuteHardwareManager hardwareManager, ISystemMuteControl systemMuteControl)
        {
            this.hardwareManager = hardwareManager;
            this.systemMuteControl = systemMuteControl;
            
            systemMuteControl.StateChanged+=MuteStateChanged;
            MuteState = systemMuteControl.GetState();

            ToggleCommand = new ActionCommand(ToggleMute);
            RefreshHardwareCommand = new ActionCommand(RefreshHardware);
        }

        private void ToggleMute()
        {
            switch (MuteState)
            {
                case MuteState.Unknown:
                    break;
                case MuteState.Muted:
                    systemMuteControl.Unmute();
                    break;
                case MuteState.Unmuted:
                    systemMuteControl.Mute();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MuteStateChanged(object? sender, MuteChangedEventArgs e)
        {
            MuteState = e.NewState;
        }

        /// <summary>
        /// Checks all available hardware.
        /// </summary>
        public void RefreshHardware()
        {
            hardwareManager.ChangeDevice(null);

            if(hardwareManager.AvailableDevices.FirstOrDefault() is { } hardware)
                hardwareManager.ChangeDevice(hardware);
        }
        
        public ICommand RefreshHardwareCommand { get; }

        public ObservableCollection<PossibleHardwareViewModel> PossibleHardware { get; }

        private MuteState muteState;

        public MuteState MuteState
        {
            get => muteState;
            set => SetProperty(ref muteState, value);
        }
        
        public ICommand ToggleCommand { get; }

    }
}