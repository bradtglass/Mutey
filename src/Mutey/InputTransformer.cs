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

        private readonly TimeSpan inputCooldown;

        private readonly object transformLock = new();
        private bool isInPttState;
        private DateTime? lastInput;
        private TransformModes modes;
        private Timer? pttActivationTimer;
        private TimeSpan smartPttActivationDuration;

        public InputTransformer()
        {
            inputCooldown = Settings.Default.InputCooldownDuration;

            Settings.Default.PropertyChanged += (_, _) => RefreshUserSettings();
            RefreshUserSettings();
        }

        private void RefreshUserSettings()
        {
            smartPttActivationDuration = Settings.Default.SmartPttActivationDuration;
            modes = Settings.Default.DefaultTransformMode;
        }

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
                        logger.Trace(
                            "Input ignored because duration since last previous was {Duration} (cooldown is {Cooldown})",
                            duration, inputCooldown);
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
                    {
                        DefaultProcessInput(hardwareType, messageType, now);
                    }
                }

                lastInput = now;
            }
        }

        private void BeginPttActivationTimer(DateTime inputTime)
        {
            pttActivationTimer?.Dispose();

            logger.Trace("Beginning smart PTT timer");
            pttActivationTimer = new Timer(TryInitiatePtt, inputTime, smartPttActivationDuration,
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

                if (!modes.HasFlag(TransformModes.Ptt))
                {
                    logger.Warn("Not activating PTT, it is not enabled");
                    return;
                }

                ActivatePtt();
            }
        }

        private void ActivatePtt()
        {
            logger.Trace("Activating PTT");
            isInPttState = true;
            RaiseAction(MuteAction.Unmute);
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
            switch (messageType)
            {
                case HardwareMessageType.StartToggle:
                    if (modes == TransformModes.Ptt)
                    {
                        logger.Trace("Mode is only PTT, activating PTT directly from start toggle");
                        ActivatePtt();
                        return;
                    }

                    if (modes.HasFlag(TransformModes.Ptt))
                        BeginPttActivationTimer(inputTime);

                    if (modes.HasFlag(TransformModes.Toggle))
                    {
                        logger.Trace("Raising toggle action");
                        RaiseAction(MuteAction.Toggle);
                    }

                    return;
                case HardwareMessageType.EndToggle:
                    pttActivationTimer?.Dispose();
                    pttActivationTimer = null;
                    logger.Trace("Message of end toggle requires no action");

                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}