namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class MuteDeviceManager : IMuteDeviceManager
    {
        private readonly List<KeyValuePair<IMuteDeviceDetector, PossibleMuteDevice>> devices = new();

        public event EventHandler<CurrentDeviceChangedEventArgs>? CurrentDeviceChanged;

        public event EventHandler<EventArgs>? AvailableDevicesChanged;
        public IMuteDevice? CurrentDevice { get; private set; }

        public IEnumerable<PossibleMuteDevice> AvailableDevices
        {
            get
            {
                lock ( devices )
                {
                    return devices.Select( d => d.Value ).ToList();
                }
            }
        }

        public void ChangeDevice( PossibleMuteDevice? newDevice )
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

        public void RegisterDeviceDetector( IMuteDeviceDetector detector )
        {
            detector.DevicesChanged += ( sender, _ ) => RegisterChangedDevices( (IMuteDeviceDetector) sender! );
            RegisterChangedDevices( detector );
        }

        private void RegisterChangedDevices( IMuteDeviceDetector detector )
        {
            List<PossibleMuteDevice> oldDevices = new();
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
                                                                  => PossibleMuteDevice.Comparer.Equals( d, device ) );

                    KeyValuePair<IMuteDeviceDetector, PossibleMuteDevice> newItem = new(detector, device);

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
                         oldDevices.Any( d => PossibleMuteDevice.Comparer.Equals( d, CurrentDevice.Source ) ) )
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
