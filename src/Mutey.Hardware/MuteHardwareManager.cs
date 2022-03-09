namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class MuteHardwareManager : IMuteHardwareManager
    {
        private readonly List<KeyValuePair<IMuteHardwareDetector, PossibleMuteHardware>> devices = new();

        public event EventHandler<CurrentDeviceChangedEventArgs>? CurrentDeviceChanged;

        public event EventHandler<EventArgs>? AvailableDevicesChanged;
        public IMuteHardware? CurrentDevice { get; private set; }

        public IEnumerable<PossibleMuteHardware> AvailableDevices
        {
            get
            {
                lock ( devices )
                {
                    return devices.Select( d => d.Value ).ToList();
                }
            }
        }

        public void ChangeDevice( PossibleMuteHardware? newDevice )
        {
            if ( CurrentDevice == null && newDevice == null )
            {
                return;
            }

            var oldHardware = CurrentDevice;
            if ( oldHardware is IDisposable disposable )
            {
                disposable.Dispose();
            }

            var newHardware = newDevice?.Connect();

            CurrentDevice = newHardware;

            CurrentDeviceChanged?.Invoke( this, new CurrentDeviceChangedEventArgs( oldHardware, newHardware ) );
        }

        public void RegisterHardwareDetector( IMuteHardwareDetector detector )
        {
            detector.HardwareChanged += ( sender, _ ) => RegisterChangedDevices( (IMuteHardwareDetector) sender! );
            RegisterChangedDevices( detector );
        }

        private void RegisterChangedDevices( IMuteHardwareDetector detector )
        {
            List<PossibleMuteHardware> oldDevices = new();
            var changed = false;
            lock ( devices )
            {
                for ( int i = devices.Count - 1; i >= 0; i-- )
                {
                    if ( ReferenceEquals( devices[ i ].Key, detector ) )
                    {
                        oldDevices.Add( devices[ i ].Value );
                        devices.RemoveAt( i );
                    }
                }

                foreach ( var device in detector.Find() )
                {
                    int existingIndex = oldDevices.FindIndex( d
                                                                  => PossibleMuteHardware.Comparer.Equals( d, device ) );

                    KeyValuePair<IMuteHardwareDetector, PossibleMuteHardware> newItem = new(detector, device);

                    if ( existingIndex < 0 )
                    {
                        changed = true;
                        devices.Add( newItem );
                    }
                    else
                    {
                        oldDevices.RemoveAt( existingIndex );
                        devices.Add( newItem );
                    }
                }

                if ( oldDevices.Count > 0 )
                {
                    changed = true;

                    if ( CurrentDevice != null &&
                         oldDevices.Any( d => PossibleMuteHardware.Comparer.Equals( d, CurrentDevice.Source ) ) )
                    {
                        ChangeDevice( null );
                    }
                }
            }

            if ( changed )
            {
                AvailableDevicesChanged?.Invoke( this, EventArgs.Empty );
            }
        }
    }
}
