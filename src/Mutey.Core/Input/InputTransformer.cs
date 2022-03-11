namespace Mutey.Core.Input
{
    using System;
    using System.Threading;
    using Mutey.Core.Audio;
    using Mutey.Core.Settings;
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
            inputCooldown = settingsStore.Get<InputSettings>().InputCooldownDuration;

            settingsStore.RegisterForNotifications<InputSettings>( RefreshUserSettings );
            RefreshUserSettings( settingsStore.Get<InputSettings>() );
        }

        private void RefreshUserSettings( InputSettings settings )
        {
            smartPttActivationDuration = settings.SmartPttActivationDuration;
            // ReSharper disable once InconsistentlySynchronizedField
            modes = settings.DefaultTransformMode;
        }

        public TransformedInput Transform( InputMessageKind messageKind )
        {
            var now = DateTime.Now;

            lock ( transformLock )
            {
                logger.Trace( "Received input '{Input}' at '{Timetamp}'", messageKind, now );
                if ( lastInput is not { } validLastInput )
                {
                    lastInput = now;

                    return DefaultProcessInput( messageKind, now );
                }

                lastInput = now;
                var duration = now - validLastInput;

                if ( duration < inputCooldown )
                {
                    logger.Trace(
                                 "Input ignored because duration since last previous was {Duration} (cooldown is {Cooldown})",
                                 duration, inputCooldown );

                    return TransformedInput.None;
                }

                if ( isInPttState &&
                     messageKind == InputMessageKind.EndToggle )
                {
                    logger.Trace( "End of PTT detected, raising mute action" );
                    isInPttState = false;

                    return new TransformedInput( MuteAction.Mute, false );
                }

                return DefaultProcessInput( messageKind, now );
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

        private TransformedInput ActivatePtt()
        {
            logger.Trace( "Activating PTT" );
            isInPttState = true;
            
            return new TransformedInput( MuteAction.Unmute, true );
        }

        private TransformedInput DefaultProcessInput( InputMessageKind messageKind, DateTime inputTime )
        {
            if ( messageKind == InputMessageKind.Unknown )
            {
                logger.Warn( "Cannot process input for unknown message type" );
                
                return TransformedInput.None;
            }

            isInPttState = false;
            switch ( messageKind )
            {
                case InputMessageKind.StartToggle:
                    if ( modes == TransformModes.Ptt )
                    {
                        logger.Trace( "Mode is only PTT, activating PTT directly from start toggle" );
                        
                        return ActivatePtt();
                    }

                    if ( modes.HasFlag( TransformModes.Ptt ) )
                    {
                        BeginPttActivationTimer( inputTime );
                    }

                    if ( modes.HasFlag( TransformModes.Toggle ) )
                    {
                        logger.Trace( "Raising toggle action" );
                        return new TransformedInput( MuteAction.Toggle, false );
                    }

                    return TransformedInput.None;
                case InputMessageKind.EndToggle:
                    pttActivationTimer?.Dispose();
                    pttActivationTimer = null;
                    logger.Trace( "Message of end toggle requires no action" );

                    return TransformedInput.None;
                default:
                    throw new ArgumentOutOfRangeException( nameof( messageKind ), messageKind, null );
            }
        }
    }
}
