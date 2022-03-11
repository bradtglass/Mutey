namespace Mutey.Hardware.Serial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using Microsoft.Win32;

    public sealed class SerialDeviceDetector : IMuteDeviceDetector, IDisposable
    {
        private readonly ManagementEventWatcher watcher =
            new(new WqlEventQuery( "SELECT * FROM Win32_DeviceChangeEvent" ));

        public SerialDeviceDetector()
        {
            watcher.EventArrived += DeviceChanged;
            watcher.Start();
        }

        public void Dispose()
        {
            watcher.Dispose();
        }

        public IEnumerable<PossibleMuteDevice> Find()
        {
            // Modified from https://stackoverflow.com/a/64541160/9766035
            using ManagementClass entity = new("Win32_PnPEntity");

            foreach ( var instance in entity.GetInstances()
                                            .OfType<ManagementObject>() )
            {
                object guid = instance.GetPropertyValue( "ClassGuid" );
                if ( guid == null || guid.ToString()?.ToUpper() != "{4D36E978-E325-11CE-BFC1-08002BE10318}" )
                {
                    continue; // Skip all devices except device class "PORTS"
                }

                var description = (string) instance.GetPropertyValue( "Caption" );
                var manufacturer = (string) instance.GetPropertyValue( "Manufacturer" );
                var deviceId = (string) instance.GetPropertyValue( "PnpDeviceID" );
                // ReSharper disable once StringLiteralTypo
                string registryPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Enum\\" +
                                      deviceId +
                                      "\\Device Parameters";
                var portName = (string) ( Registry.GetValue( registryPath, "PortName", "" ) ?? throw new InvalidOperationException( "Port name cannot be null" ) );

                int comPosition = description.IndexOf( " (COM", StringComparison.OrdinalIgnoreCase );
                if ( comPosition > 0 ) // remove COM port from description
                {
                    description = description.Substring( 0, comPosition );
                }

                yield return new PossibleSerialMuteDevice( portName, description, manufacturer, deviceId );
            }
        }

        public event EventHandler? DevicesChanged;

        private void DeviceChanged( object sender, EventArrivedEventArgs args )
        {
            // It may be possible to determine the scope of the change and if it effects our hardware but for now just always assume it does
            DevicesChanged?.Invoke( this, EventArgs.Empty );
        }
    }
}
