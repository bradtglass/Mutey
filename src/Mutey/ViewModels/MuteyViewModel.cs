using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Mutey.Input;
using Mutey.Mute;
using NLog;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class MuteyViewModel : BindableBase, IMutey
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IMuteHardwareManager hardwareManager;
        private readonly ISystemMuteControl systemMuteControl;
        private readonly SynchronizationContext synchronizationContext;
        private readonly InputTransformer transformer = new();

        private MuteState muteState;

        public MuteyViewModel(IMuteHardwareManager hardwareManager, ISystemMuteControl systemMuteControl)
        {
            this.hardwareManager = hardwareManager;
            this.systemMuteControl = systemMuteControl;

            synchronizationContext = SynchronizationContext.Current ??
                                     throw new InvalidOperationException("Failed to get synchronization context");

            transformer.ActionRequired += OnTransformedActionRequired;

            systemMuteControl.StateChanged += MuteStateChanged;
            MuteState = systemMuteControl.GetState();

            hardwareManager.AvailableDevicesChanged +=
                (_, _) => synchronizationContext.Post(_ => RefreshHardware(), null);
            hardwareManager.CurrentDeviceChanged += CurrentInputDeviceChanged;

            ToggleCommand = new ActionCommand(ToggleMuteByCommand);
            RefreshHardwareCommand = new ActionCommand(RefreshHardwareByCommand);
            ActivateHardwareCommand = new ActionCommand(ActivateHardwareByCommand);
        }

        public ICommand ActivateHardwareCommand { get; }

        public ICommand RefreshHardwareCommand { get; }

        public ObservableCollection<PossibleHardwareViewModel> PossibleHardware { get; } = new();

        public MuteState MuteState
        {
            get => muteState;
            set => SetProperty(ref muteState, value);
        }

        public ICommand ToggleCommand { get; }

        public void RegisterHardware(IMuteHardwareDetector detector)
        {
            hardwareManager.RegisterHardwareDetector(detector);
        }

        private void RefreshHardwareByCommand()
        {
            logger.Debug("User began a manual hardware refresh");
            RefreshHardware();
        }

        /// <summary>
        ///     Checks all available hardware.
        /// </summary>
        public void RefreshHardware()
        {
            hardwareManager.ChangeDevice(null);

            string? previousSelection = PossibleHardware.FirstOrDefault(h => h.IsActive)?.Name;

            PossibleHardware.Clear();
            foreach (PossibleMuteHardware device in hardwareManager.AvailableDevices)
                PossibleHardware.Add(new PossibleHardwareViewModel(device.FriendlyName, device.Type));

            if (previousSelection != null)
            {
                PossibleHardwareViewModel?
                    viewModel = PossibleHardware.FirstOrDefault(h => h.Name == previousSelection);
                if (viewModel != null)
                    ActivateHardwareByCommand(viewModel);
            }
        }

        private void OnTransformedActionRequired(object? sender, MuteAction e)
        {
            switch (e)
            {
                case MuteAction.Mute:
                    synchronizationContext.Send(_ => systemMuteControl.Mute(), null);
                    break;
                case MuteAction.Unmute:
                    synchronizationContext.Send(_ => systemMuteControl.Unmute(), null);
                    break;
                case MuteAction.Toggle:
                    synchronizationContext.Send(_ => ToggleMute(), null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }

        private void CurrentInputDeviceChanged(object? sender, CurrentDeviceChangedEventArgs e)
        {
            if (e.OldDevice is { } oldDevice)
                oldDevice.MessageReceived -= InputDeviceOnMessageReceived;

            if (e.NewDevice is { } newDevice)
                newDevice.MessageReceived += InputDeviceOnMessageReceived;
        }

        private void InputDeviceOnMessageReceived(object? sender, HardwareMessageReceivedEventArgs e)
            => transformer.Transform(e.Hardware, e.Message);

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

        private void ToggleMuteByCommand()
        {
            logger.Debug("User invoked manual mute toggle command");
            ToggleMute();
        }

        private void MuteStateChanged(object? sender, MuteChangedEventArgs e)
        {
            MuteState = e.NewState;
        }

        private void ActivateHardwareByCommand(object parameter)
        {
            logger.Debug("User activated a new input hardware");
            
            if (parameter is not PossibleHardwareViewModel viewModel)
            {
                logger.Error("Invalid parameter type to activate hardware: {Parameter}", parameter);
                return;
            }
            
            ActivateHardware(viewModel);
        }
        
        private void ActivateHardware(PossibleHardwareViewModel viewModel)
        {
            if(viewModel.IsActive)
                return;
            
            PossibleMuteHardware? hardware =
                hardwareManager.AvailableDevices.FirstOrDefault(d => d.FriendlyName == viewModel.Name);

            if (hardware == null)
            {
                logger.Error("Failed to activate hardware {Name}, not found in hardware manager", viewModel.Name);
                return;
            }

            hardwareManager.ChangeDevice(hardware);

            foreach (PossibleHardwareViewModel hardwareViewModel in PossibleHardware)
                hardwareViewModel.IsActive = false;

            viewModel.IsActive = true;
        }
    }
}