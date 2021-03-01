using System;
using Mutey.Input;
using NLog;

namespace Mutey
{
    /// <summary>
    ///     Transforms the input messages into actions to be sent to active calls.
    /// </summary>
    public class InputTransformer
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static readonly TimeSpan DefaultInputCooldown = TimeSpan.FromMilliseconds(50);

        private readonly TimeSpan inputCooldown;

        private Input? lastInput;
        private readonly object transformLock = new();

        public InputTransformer(TimeSpan inputCooldown)
        {
            this.inputCooldown = inputCooldown;
        }

        public bool SmartPtt { get; set; } = true;

        public TimeSpan SmartPttActivationDuration { get; set; } = TimeSpan.FromSeconds(0.35);
        
        public event EventHandler<MuteAction>? ActionRequired;

        public void Transform(HardwareType hardwareType, HardwareMessageType messageType)
        {
            DateTime now = DateTime.Now;

            lock (transformLock)
            {
                logger.Trace("Received input '{Input}' at '{Timetamp}'", messageType, now);
                if (lastInput is not { } validLastInput)
                {
                    ProcessFirstInput(hardwareType, messageType);
                }
                else
                {
                    TimeSpan duration = now - validLastInput.Timestamp;

                    if (duration < inputCooldown)
                    {
                        logger.Trace("Input ignored because duration since last previous was {Duration} (cooldown is {Cooldown})", duration, inputCooldown);
                        return;
                    }

                    if (validLastInput.HardwareType == HardwareType.Toggle &&
                        hardwareType == HardwareType.Toggle &&
                        validLastInput.MessageType == HardwareMessageType.StartToggle &&
                        messageType == HardwareMessageType.EndToggle &&
                        duration > SmartPttActivationDuration)
                    {
                        logger.Trace("End of PTT detected, raising mute action");
                        RaiseAction(MuteAction.Mute);
                    }
                    else
                        ProcessFirstInput(hardwareType, messageType);
                }

                lastInput = new Input(hardwareType, messageType, now);
            }
        }

        private void RaiseAction(MuteAction action)
            => ActionRequired?.Invoke(this, action);

        private void ProcessFirstInput(HardwareType hardwareType, HardwareMessageType messageType)
        {
            if (hardwareType == HardwareType.Unknown)
            {
                logger.Warn("Cannot process input for unknown hardware");
                return;
            }

            if (messageType == HardwareMessageType.Unknown)
            {
                logger.Warn("Cannot process input for unknown message type");
                return;
            }
            
            switch (hardwareType)
            {
                case HardwareType.Toggle:
                    switch (messageType)
                    {
                        case HardwareMessageType.StartToggle:
                            logger.Trace("Raising toggle action");
                            RaiseAction(MuteAction.Toggle);
                            return;
                        case HardwareMessageType.EndToggle:
                            logger.Trace("Message of end toggle requires no action");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(hardwareType), hardwareType, null);
            }
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