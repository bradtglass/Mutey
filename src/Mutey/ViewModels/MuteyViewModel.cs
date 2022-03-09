namespace Mutey.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors.Core;
    using Mutey.Audio;
    using Mutey.Audio.Mute;
    using Mutey.Hardware;
    using Mutey.Popup;
    using NLog;
    using Prism.Mvvm;

    /// <summary>
    ///     This is the primary place that all of the business logic is pulled together, input devices, software inputs, popups
    ///     and input transformation.
    /// </summary>
    internal class MuteyViewModel : BindableBase, IMutey
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IMuteHardwareManager hardwareManager;
        private readonly MicStatePopupManager popupManager;
        private readonly SynchronizationContext synchronizationContext;
        private readonly ISystemMuteControl systemMuteControl;
        private readonly InputTransformer transformer = new();

        private MuteState muteState;

        public ICommand ToggleHardwareCommand { get; }

        public ICommand RefreshHardwareCommand { get; }

        public ObservableCollection<PossibleHardwareViewModel> PossibleHardware { get; } = new();

        public MuteState MuteState
        {
            get => muteState;
            set => SetProperty( ref muteState, value );
        }

        public ICommand ToggleCommand { get; }

        public MuteyViewModel( IMuteHardwareManager hardwareManager, ISystemMuteControl systemMuteControl, MicStatePopupManager popupManager )
        {
            this.hardwareManager = hardwareManager;
            this.systemMuteControl = systemMuteControl;
            this.popupManager = popupManager;

            synchronizationContext = SynchronizationContext.Current ??
                                     throw new InvalidOperationException( "Failed to get synchronization context" );

            transformer.Transformed += OnTransformedActionRequired;

            systemMuteControl.StateChanged += MuteStateChanged;
            MuteState = systemMuteControl.GetState();
            popupManager.PopupPressed += ( _, _ ) => ToggleMuteByCommand();
            popupManager.BeginLifetime().ChangeState( MuteState );

            hardwareManager.AvailableDevicesChanged +=
                ( _, _ ) => synchronizationContext.Post( _ => RefreshHardware(), null );
            hardwareManager.CurrentDeviceChanged += CurrentInputDeviceChanged;

            ToggleCommand = new ActionCommand( ToggleMuteByCommand );
            RefreshHardwareCommand = new ActionCommand( RefreshHardwareByCommand );
            ToggleHardwareCommand = new ActionCommand( ToggleHardwareByCommand );
        }

        public void RegisterHardware( IMuteHardwareDetector detector )
        {
            hardwareManager.RegisterHardwareDetector( detector );
        }

        /// <summary>
        ///     Checks all available hardware.
        /// </summary>
        public void RefreshHardware()
        {
            string? previousSelection = PossibleHardware.FirstOrDefault( h => h.IsActive )?.Id ??
                                        Settings.Default.LastDeviceId;

            PossibleHardware.Clear();
            foreach ( var device in hardwareManager.AvailableDevices )
            {
                PossibleHardware.Add( new PossibleHardwareViewModel( device.FriendlyName, device.Type, device.LocalIdentifier ) );
            }

            if ( previousSelection != null )
            {
                var
                    viewModel = PossibleHardware.FirstOrDefault( h => h.Id == previousSelection );

                if ( viewModel != null )
                {
                    ChangeHardwareState( viewModel, true );
                }
                else
                {
                    hardwareManager.ChangeDevice( null );
                }
            }
        }

        private void RefreshHardwareByCommand()
        {
            logger.Debug( "User began a manual hardware refresh" );
            RefreshHardware();
        }

        private void OnTransformedActionRequired( object? sender, TransformedMuteOutputEventArgs e )
        {
            switch ( e.Action )
            {
                case MuteAction.Mute:
                    synchronizationContext.Send( _ => systemMuteControl.Mute(), null );
                    break;
                case MuteAction.Unmute:
                    synchronizationContext.Send( _ => systemMuteControl.Unmute(), null );
                    break;
                case MuteAction.Toggle:
                    synchronizationContext.Send( _ => ToggleMute(), null );
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( e ), e, null );
            }

            var currentState = systemMuteControl.GetState();
            if ( e.IsInPtt )
            {
                popupManager.Show( currentState );
            }
            else
            {
                popupManager.Flash( currentState );
            }
        }

        private void CurrentInputDeviceChanged( object? sender, CurrentDeviceChangedEventArgs e )
        {
            if ( e.OldDevice is { } oldDevice )
            {
                oldDevice.MessageReceived -= InputDeviceOnMessageReceived;
            }

            if ( e.NewDevice is { } newDevice )
            {
                newDevice.MessageReceived += InputDeviceOnMessageReceived;
            }
        }

        private void InputDeviceOnMessageReceived( object? sender, HardwareMessageReceivedEventArgs e )
        {
            transformer.Transform( e.Hardware, e.Message );
        }

        private void ToggleMute()
        {
            switch ( MuteState )
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
            logger.Debug( "User invoked manual mute toggle command" );
            ToggleMute();
            popupManager.Flash( systemMuteControl.GetState() );
        }

        private void MuteStateChanged( object? sender, MuteChangedEventArgs e )
        {
            MuteState = e.NewState;
        }

        private void ToggleHardwareByCommand( object parameter )
        {
            logger.Debug( "User toggled the active hardware" );

            if ( parameter is not PossibleHardwareViewModel viewModel )
            {
                logger.Error( "Invalid parameter type to toggle hardware: {Parameter}", parameter );
                return;
            }

            ChangeHardwareState( viewModel, !viewModel.IsActive );
        }

        private void ChangeHardwareState( PossibleHardwareViewModel viewModel, bool newState )
        {
            if ( viewModel.IsActive == newState )
            {
                return;
            }

            viewModel.IsActive = newState;
            foreach ( var hardwareViewModel in PossibleHardware.Where( h => !ReferenceEquals( h, viewModel ) ) )
            {
                hardwareViewModel.IsActive = false;
            }

            var hardware =
                hardwareManager.AvailableDevices.FirstOrDefault( d => d.FriendlyName == viewModel.Name );

            if ( hardware == null )
            {
                logger.Error( "Failed to activate hardware {Name}, not found in hardware manager", viewModel.Name );
                return;
            }

            if ( newState )
            {
                logger.Debug( "Activating new device: {Device}", hardware.LocalIdentifier );
                hardwareManager.ChangeDevice( hardware );

                Settings.Default.LastDeviceId = viewModel.Id;
                Settings.Default.Save();
            }
            else
            {
                logger.Debug( "Deactivating current device" );
                hardwareManager.ChangeDevice( null );

                if ( viewModel.Id == Settings.Default.LastDeviceId )
                {
                    Settings.Default.LastDeviceId = null;
                    Settings.Default.Save();
                }
            }
        }
    }
}
