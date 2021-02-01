using System;

namespace Mutey.Input
{
    public class SerialMuteButtonSettings
    {
        public SerialMuteButtonSettings(string port)
        {
            Port = port;
        }

        public int BaudRate { get; init; } = 57600;
        public string Port { get; }
    }
}