namespace Mutey.Input
{
    public class PossibleSerialMuteHardware : PossibleMuteHardware
    {
        public PossibleSerialMuteHardware(string port, string description, string manufacturer, string deviceId)
        {
            Port = port;
            Description = description;
            Manufacturer = manufacturer;
            DeviceId = deviceId;
        }

        public int BaudRate { get; init; } = 57600;
        public string Port { get; }
        public string Description { get; }
        public string Manufacturer { get; }
        public string DeviceId { get; }
        public override string Name => Port;
        public override string Type { get; } = "Serial";
        public override bool IsPresumptive { get; } = true;

        public override IMuteHardware Connect()
            => new SerialMuteHardware(this);
    }
}