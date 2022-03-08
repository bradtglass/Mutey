using System.Diagnostics;

namespace Mutey.Input
{
    [DebuggerDisplay("{Port}: {Description}")]
    public class PossibleSerialMuteHardware : PossibleMuteHardware
    {
        public PossibleSerialMuteHardware(string port, string description, string manufacturer, string deviceId)
        {
            Port = port;
            Description = description;

            LocalIdentifier = $"{Port}:{manufacturer}:{deviceId}";
        }

        public int BaudRate { get; } = 57600;

        public string Port { get; }

        public string Description { get; }

        public override string FriendlyName => Port;

        public override string Type { get; } = "Serial";

        public override bool IsPresumptive { get; } = true;

        public override string LocalIdentifier { get; }

        public override IMuteHardware Connect()
            => new SerialMuteHardware(this);
    }
}