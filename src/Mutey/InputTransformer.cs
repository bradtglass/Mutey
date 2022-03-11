namespace Mutey
{
    using System;
    using System.Threading;
    using Mutey.Core.Settings;
    using Mutey.Hardware;
    using NLog;

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

        public InputTransformer( ISettingsStore settingsStore )
        {
            inputCooldown = settingsStore.Get<MuteySettings>().InputCooldownDuration;

            settingsStore.RegisterForNotifications<MuteySettings>( SettingsChanged );
            RefreshUserSettings( settingsStore.Get<MuteySettings>() );
        }

        private void SettingsChanged( SettingsChangedEventArgs<MuteySettings> args )
            => RefreshUserSettings( args.NewValue );
        
        private void RefreshUserSettings( MuteySettings settings )
        {
            smartPttActivationDuration = settings.SmartPttActivationDuration;
            // ReSharper disable once InconsistentlySynchronizedField
            modes = settings.DefaultTransformMode;
        }

        public event EventHandler<TransformedMuteOutputEventArgs>? Transformed;

        public void Transform( DeviceKind deviceKind, InputMessageKind messageKind )
        {
            var now = DateTime.Now;

            lock ( transformLock )
            {
                logger.Trace( "Received input '{Input}' at '{Timetamp}'", messageKind, now );
                if ( lastInput is not { } validLastInput )
                {
                    DefaultProcessInput( deviceKind, messageKind, now );
                }
                else
                {
                    var duration = now - validLastInput;

                    if ( duration < inputCooldown )
                    {
                        logger.Trace(
                                     "Input ignored because duration since last previous was {Duration} (cooldown is {Cooldown})",
                                     duration, inputCooldown );
                        return;
                    }

                    if ( isInPttState &&
                         messageKind == InputMessageKind.EndToggle )
                    {
                        logger.Trace( "End of PTT detected, raising mute action" );
                        isInPttState = false;
                        RaiseTransformed( MuteAction.Mute, false );
                    }
                    else
                    {
                        DefaultProcessInput( deviceKind, messageKind, now );
                    }
                }

                lastInput = now;
            }
        }

        private void BeginPttActivationTimer( DateTime inputTime )
        {
            pttActivationTimer?.Dispose();

            logger.Trace( "Beginning smart PTT timer" );
            pttActivationTimer = new Timer( TryInitiatePtt, inputTime, smartPttActivationDuration,
                                            Timeout.InfiniteTimeSpan );
        }

        private void TryInitiatePtt( object? state )
        {
            var inputTime = (DateTime) state!;

            lock ( transformLock )
            {
                if ( lastInput == null || inputTime != lastInput.Value )
                {
                    logger.Trace( "Not activating PTT, last input does not match expected" );
                    return;
                }

                if ( !modes.HasFlag( TransformModes.Ptt ) )
                {
                    logger.Warn( "Not activating PTT, it is not enabled" );
                    return;
                }

                ActivatePtt();
            }
        }

        private void ActivatePtt()
        {
            logger.Trace( "Activating PTT" );
            isInPttState = true;
            RaiseTransformed( MuteAction.Unmute, true );
        }

        private void RaiseTransformed( MuteAction action, bool ptt )
        {
            Transformed?.Invoke( this, new TransformedMuteOutputEventArgs( action, ptt ) );
        }

        private void DefaultProcessInput( DeviceKind deviceKind, InputMessageKind messageKind, DateTime inputTime )
        {
            if ( deviceKind == DeviceKind.Unknown )
            {
                logger.Warn( "Cannot process input for unknown hardware" );
                return;
            }

            if ( messageKind == InputMessageKind.Unknown )
            {
                logger.Warn( "Cannot process input for unknown message type" );
                return;
            }

            isInPttState = false;
            switch ( messageKind )
            {
                case InputMessageKind.StartToggle:
                    if ( modes == TransformModes.Ptt )
                    {
                        logger.Trace( "Mode is only PTT, activating PTT directly from start toggle" );
                        ActivatePtt();
                        return;
                    }

                    if ( modes.HasFlag( TransformModes.Ptt ) )
                    {
                        BeginPttActivationTimer( inputTime );
                    }

                    if ( modes.HasFlag( TransformModes.Toggle ) )
                    {
                        logger.Trace( "Raising toggle action" );
                        RaiseTransformed( MuteAction.Toggle, false );
                    }

                    return;
                case InputMessageKind.EndToggle:
                    pttActivationTimer?.Dispose();
                    pttActivationTimer = null;
                    logger.Trace( "Message of end toggle requires no action" );

                    return;
                default:
                    throw new ArgumentOutOfRangeException( nameof( messageKind ), messageKind, null );
            }
        }
    }
}
