using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly InputTransformer transformer = new(InputTransformer.DefaultInputCooldown);

        private MuteState muteState;

        public MuteyViewModel(IMuteHardwareManager hardwareManager, ISystemMuteControl systemMuteControl)
        {
            this.hardwareManager = hardwareManager;
            this.systemMuteControl = systemMuteControl;

            transformer.ActionRequired += OnTransformedActionRequired;

            systemMuteControl.StateChanged += MuteStateChanged;
            MuteState = systemMuteControl.GetState();

            hardwareManager.AvailableDevicesChanged += (_, _) => RefreshHardware();
            hardwareManager.CurrentDeviceChanged += CurrentInputDeviceChanged;

            ToggleCommand = new ActionCommand(ToggleMute);
            RefreshHardwareCommand = new ActionCommand(RefreshHardware);
            ActivateHardwareCommand = new ActionCommand(ActivateHardware);
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

        /// <summary>
        ///     Checks all available hardware.
        /// </summary>
        public void RefreshHardware()
        {
            hardwareManager.ChangeDevice(null);

            string? previousSelection = PossibleHardware.FirstOrDefault(h => h.IsActive)?.Name;

            PossibleHardware.Clear();
            foreach (PossibleMuteHardware device in hardwareManager.AvailableDevices)
                PossibleHardware.Add(new PossibleHardwareViewModel(device.Name, device.Type));

            if (previousSelection != null)
            {
                PossibleHardwareViewModel?
                    viewModel = PossibleHardware.FirstOrDefault(h => h.Name == previousSelection);
                if (viewModel != null)
                    ActivateHardware(viewModel);
            }
        }

        private void OnTransformedActionRequired(object? sender, MuteAction e)
        {
            switch (e)
            {
                case MuteAction.Mute:
                    systemMuteControl.Mute();
                    break;
                case MuteAction.Unmute:
                    systemMuteControl.Unmute();
                    break;
                case MuteAction.Toggle:
                    ToggleMute();
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

        private void MuteStateChanged(object? sender, MuteChangedEventArgs e)
        {
            MuteState = e.NewState;
        }

        public void ActivateHardware(object value)
        {
            PossibleHardwareViewModel? viewModel = value as PossibleHardwareViewModel;

            if (viewModel == null)
            {
                logger.Error("Value passed to method ({Value}) is not a view model", value);
                return;
            }

            PossibleMuteHardware? hardware =
                hardwareManager.AvailableDevices.FirstOrDefault(d => d.Name == viewModel.Name);

            if (hardware == null)
            {
                logger.Error("Failed to activate hardware {Name}, not found in hardware manager", viewModel.Name);
                return;
            }

            hardwareManager.ChangeDevice(hardware);
        }
    }
}