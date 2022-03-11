namespace Mutey.Hardware.Serial
{
    using System.Diagnostics;

    [ DebuggerDisplay( "{Port}: {Description}" ) ]
    public class PossibleSerialMuteDevice : PossibleMuteDevice
    {
        public int BaudRate { get; } = 57600;

        public string Port { get; }

        public string Description { get; }

        public override string FriendlyName => Port;

        public override string Type { get; } = "Serial";

        public override bool IsPresumptive { get; } = true;

        public override string LocalIdentifier { get; }

        public PossibleSerialMuteDevice( string port, string description, string manufacturer, string deviceId )
        {
            Port = port;
            Description = description;

            LocalIdentifier = $"{Port}:{manufacturer}:{deviceId}";
        }

        public override IMuteDevice Connect()
        {
            return new SerialMuteDevice( this );
        }
    }
}
