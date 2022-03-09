namespace Mutey.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using Microsoft.Win32;

    public sealed class SerialHardwareDetector : IMuteHardwareDetector, IDisposable
    {
        private readonly ManagementEventWatcher watcher =
            new(new WqlEventQuery( "SELECT * FROM Win32_DeviceChangeEvent" ));

        public SerialHardwareDetector()
        {
            watcher.EventArrived += DeviceChanged;
            watcher.Start();
        }

        public void Dispose()
        {
            watcher.Dispose();
        }

        public IEnumerable<PossibleMuteHardware> Find()
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

                yield return new PossibleSerialMuteHardware( portName, description, manufacturer, deviceId );
            }
        }

        public event EventHandler? HardwareChanged;

        private void DeviceChanged( object sender, EventArrivedEventArgs args )
        {
            // It may be possible to determine the scope of the change and if it effects our hardware but for now just always assume it does
            HardwareChanged?.Invoke( this, EventArgs.Empty );
        }
    }
}
