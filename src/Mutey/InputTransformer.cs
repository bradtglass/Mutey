using System;
using System.Threading;
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

        private readonly object transformLock = new();
        private DateTime? lastInput;
        private Timer? pttActivationTimer;
        private bool isInPttState = false;

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
                    DefaultProcessInput(hardwareType, messageType, now);
                }
                else
                {
                    TimeSpan duration = now - validLastInput;

                    if (duration < inputCooldown)
                    {
                        logger.Trace("Input ignored because duration since last previous was {Duration} (cooldown is {Cooldown})", duration, inputCooldown);
                        return;
                    }

                    if (isInPttState &&
                        messageType == HardwareMessageType.EndToggle)
                    {
                        logger.Trace("End of PTT detected, raising mute action");
                        isInPttState = false;
                        RaiseAction(MuteAction.Mute);
                    }
                    else
                        DefaultProcessInput(hardwareType, messageType, now);
                }

                lastInput = now;
            }
        }

        private void BeginPttActivationTimer(DateTime inputTime)
        {
            pttActivationTimer?.Dispose();

            pttActivationTimer = new Timer(TryInitiatePtt, inputTime, SmartPttActivationDuration,
                Timeout.InfiniteTimeSpan);
        }

        private void TryInitiatePtt(object? state)
        {
            DateTime inputTime = (DateTime) state!;

            lock (transformLock)
            {
                if (lastInput == null || inputTime != lastInput.Value)
                {
                    logger.Trace("Not activating PTT, last input does not match expected");
                    return;
                }

                logger.Trace("Activating PTT");
                isInPttState = true;
                RaiseAction(MuteAction.Unmute);
            }
        }

        private void RaiseAction(MuteAction action)
            => ActionRequired?.Invoke(this, action);

        private void DefaultProcessInput(HardwareType hardwareType, HardwareMessageType messageType, DateTime inputTime)
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

            isInPttState = false;
            switch (hardwareType)
            {
                case HardwareType.Toggle:
                    switch (messageType)
                    {
                        case HardwareMessageType.StartToggle:
                            logger.Trace("Raising toggle action");
                            BeginPttActivationTimer(inputTime);
                            RaiseAction(MuteAction.Toggle);
                            return;
                        case HardwareMessageType.EndToggle:
                            pttActivationTimer?.Dispose();
                            pttActivationTimer = null;
                            logger.Trace("Message of end toggle requires no action");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(hardwareType), hardwareType, null);
            }
        }
    }
}