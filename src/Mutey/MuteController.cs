using System;
using Mutey.Input;
using NLog;

namespace Mutey
{
    public class MuteController
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static TimeSpan DefaultInputCooldown = TimeSpan.FromMilliseconds(50);

        private readonly TimeSpan inputCooldown;

        private Input? lastInput;

        public MuteController(TimeSpan inputCooldown)
        {
            this.inputCooldown = inputCooldown;
        }

        public bool SmartPtt { get; set; } = true;

        public TimeSpan SmartPttActivationDuration { get; set; } = TimeSpan.FromSeconds(1.5);

        public MuteAction ProcessInput(HardwareType hardwareType, HardwareMessageType messageType)
        {
            DateTime now = DateTime.Now;
            MuteAction action;

            if (lastInput is not { } validLastInput)
            {
                action = ProcessFirstInput(hardwareType, messageType);
            }
            else
            {
                TimeSpan duration = validLastInput.Timestamp - now;

                if (duration < inputCooldown)
                    return MuteAction.None;

                if (validLastInput.HardwareType == HardwareType.Toggle &&
                    hardwareType == HardwareType.Toggle &&
                    validLastInput.MessageType == HardwareMessageType.StartToggle &&
                    messageType == HardwareMessageType.EndToggle &&
                    duration > SmartPttActivationDuration)
                    action = MuteAction.Mute;
                else
                    action = ProcessFirstInput(hardwareType, messageType);
            }

            lastInput = new Input(hardwareType, messageType, now);
            return action;
        }

        private static MuteAction ProcessFirstInput(HardwareType hardwareType, HardwareMessageType messageType)
        {
            if (hardwareType == HardwareType.Unknown)
            {
                logger.Warn("Cannot process input for unknown hardware");
                return MuteAction.None;
            }

            if (messageType == HardwareMessageType.Unknown)
            {
                logger.Warn("Cannot process input for unknown message type");
                return MuteAction.None;
            }

            return hardwareType switch
            {
                HardwareType.Toggle => messageType switch
                {
                    HardwareMessageType.StartToggle => MuteAction.Toggle,
                    HardwareMessageType.EndToggle => MuteAction.None,
                    _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(hardwareType), hardwareType, null)
            };
        }

        private readonly struct Input
        {
            public Input(HardwareType hardwareType, HardwareMessageType messageType, DateTime timestamp)
            {
                HardwareType = hardwareType;
                MessageType = messageType;
                Timestamp = timestamp;
            }

            public HardwareType HardwareType { get; }
            public HardwareMessageType MessageType { get; }
            public DateTime Timestamp { get; }
        }
    }
}