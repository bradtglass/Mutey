namespace Mutey.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors.Core;
    using Mutey.Core;
    using Mutey.Core.Audio;
    using Mutey.Core.Input;
    using Mutey.Core.Settings;
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

        private readonly ISettingsStore settingsStore;
        private readonly IMuteDeviceManager deviceManager;
        private readonly MicStatePopupManager popupManager;
        private readonly SynchronizationContext synchronizationContext;
        private readonly ISystemMuteControl systemMuteControl;
        private readonly InputTransformer transformer;

        private MuteState muteState;

        public ICommand ToggleHardwareCommand { get; }

        public ICommand RefreshHardwareCommand { get; }

        public ObservableCollection<PossibleHardwareViewModel> PossibleDevices { get; } = new();

        public MuteState MuteState
        {
            get => muteState;
            set => SetProperty( ref muteState, value );
        }

        public ICommand ToggleCommand { get; }

        public MuteyViewModel( ISettingsStore settingsStore, IMuteDeviceManager deviceManager, ISystemMuteControl systemMuteControl, MicStatePopupManager popupManager )
        {
            this.settingsStore = settingsStore;
            this.deviceManager = deviceManager;
            this.systemMuteControl = systemMuteControl;
            this.popupManager = popupManager;
            transformer = new InputTransformer( settingsStore );

            synchronizationContext = SynchronizationContext.Current ??
                                     throw new InvalidOperationException( "Failed to get synchronization context" );

            systemMuteControl.StateChanged += MuteStateChanged;
            MuteState = systemMuteControl.GetState();
            popupManager.PopupPressed += ( _, _ ) => ToggleMuteByCommand();
            popupManager.BeginLifetime().ChangeState( MuteState );

            deviceManager.AvailableDevicesChanged +=
                ( _, _ ) => synchronizationContext.Post( _ => RefreshDevices(), null );
            deviceManager.CurrentDeviceChanged += CurrentInputDeviceChanged;

            ToggleCommand = new ActionCommand( ToggleMuteByCommand );
            RefreshHardwareCommand = new ActionCommand( RefreshHardwareByCommand );
            ToggleHardwareCommand = new ActionCommand( ToggleHardwareByCommand );
        }

        /// <summary>
        ///     Checks all available hardware.
        /// </summary>
        public void RefreshDevices()
        {
            string? previousSelection = PossibleDevices.FirstOrDefault( h => h.IsActive )?.Id ??
                                        settingsStore.Get<MuteySettings>().LastDeviceId;

            PossibleDevices.Clear();
            foreach ( var device in deviceManager.AvailableDevices )
            {
                PossibleDevices.Add( new PossibleHardwareViewModel( device.FriendlyName, device.Type, device.LocalIdentifier ) );
            }

            if ( previousSelection != null )
            {
                var
                    viewModel = PossibleDevices.FirstOrDefault( h => h.Id == previousSelection );

                if ( viewModel != null )
                {
                    ChangeHardwareState( viewModel, true );
                }
                else
                {
                    deviceManager.ChangeDevice( null );
                }
            }
        }

        private void RefreshHardwareByCommand()
        {
            logger.Debug( "User began a manual hardware refresh" );
            RefreshDevices();
        }

        private void ApplyTransformedInput( TransformedInput transformedInput )
        {
            switch ( transformedInput.Action )
            {
                case MuteAction.None:
                    return;
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
                    throw new ArgumentOutOfRangeException( nameof( transformedInput ), transformedInput, null );
            }

            var currentState = systemMuteControl.GetState();
            if ( transformedInput.IsInPtt )
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

        private void InputDeviceOnMessageReceived( object? sender, InputMessageEventArgs e )
        {
            TransformInput( e.DeviceId, e.Message );
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
            foreach ( var hardwareViewModel in PossibleDevices.Where( h => !ReferenceEquals( h, viewModel ) ) )
            {
                hardwareViewModel.IsActive = false;
            }

            var hardware =
                deviceManager.AvailableDevices.FirstOrDefault( d => d.FriendlyName == viewModel.Name );

            if ( hardware == null )
            {
                logger.Error( "Failed to activate hardware {Name}, not found in hardware manager", viewModel.Name );
                return;
            }

            if ( newState )
            {
                logger.Debug( "Activating new device: {Device}", hardware.LocalIdentifier );
                deviceManager.ChangeDevice( hardware );

                settingsStore.Set<MuteySettings>( s => s with {LastDeviceId = viewModel.Id} );
            }
            else
            {
                logger.Debug( "Deactivating current device" );
                deviceManager.ChangeDevice( null );
                
                settingsStore.Set<MuteySettings>( s => s with {LastDeviceId = null} );
            }
        }

        public void TransformInput( string deviceId, InputMessageKind messageKind )
        {
            var transformedInput = transformer.Transform( messageKind );
            
            ApplyTransformedInput(transformedInput);
        }

        public void ChangeMuteState( MuteState state )
        {
            switch ( state )
            {
                case MuteState.Muted:
                    systemMuteControl.Mute();
                    break;
                case MuteState.Unmuted:
                    systemMuteControl.Unmute();
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( state ), state, $"Cannot set state to {state}" );
            }
        }

        public event EventHandler<MuteChangedEventArgs>? StateChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}
