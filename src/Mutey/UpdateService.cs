namespace Mutey
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using NLog;

    [ UsedImplicitly ]
    public class UpdateService : IUpdateService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan delayBetweenNonErroredChecked = TimeSpan.FromHours( 1 );
        private readonly TimeSpan maxBackOff = TimeSpan.FromMinutes( 5 );

        private readonly SemaphoreSlim updateSemaphore = new(1);
        private TimeSpan lastBackOff = TimeSpan.FromSeconds( 2 );

        public UpdateService()
        {
            State = UpdateState.NotChecked;
            Task.Run( RunCheckAndQueueNextIfRequired );
        }

        public UpdateState State { get; private set; }

        public event EventHandler? StateChanged;

        public async Task<UpdateState> RefreshStateAsync()
        {
            if ( State == UpdateState.Available )
            {
                logger.Info( "Skipping update check because state is already available" );

                return UpdateState.Available;
            }

            await updateSemaphore.WaitAsync();
            try
            {
                logger.Info( "Beginning update check" );
                ChangeState( UpdateState.Checking );

                logger.Warn( "Update checking is not implemented" );

                // BG Update checking disabled due to issues with virus scanning of squirrel exe
                // Also because Squirrel.Windows does not support .NET 6
                const UpdateState newState = UpdateState.Latest;
                ChangeState( newState );

                return newState;
            }
            catch
            {
                ChangeState( UpdateState.NotChecked );
                throw;
            }
            finally
            {
                updateSemaphore.Release();
            }
        }

        public async Task UpdateAsync()
        {
            await updateSemaphore.WaitAsync();
            try
            {
                throw new NotSupportedException( "Automatic updates have been disabled" );
            }
            finally
            {
                updateSemaphore.Dispose();
            }
        }

        private async void RunCheckAndQueueNextIfRequired()
        {
            try
            {
                var refreshedState = await RefreshStateAsync();

                switch ( refreshedState )
                {
                    case UpdateState.Latest:
                        // We're currently running the latest version, queue a check for later to check again
                        logger.Info(
                                    $"Update check determined latest is installed, waiting {delayBetweenNonErroredChecked} for the next check" );

#pragma warning disable 4014
                        Task.Delay( delayBetweenNonErroredChecked ).ContinueWith( _ => RunCheckAndQueueNextIfRequired() );
#pragma warning restore 4014
                        break;
                    case UpdateState.Available:
                        // No need to carry on checking because the update will be available for as long as the application is open
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch ( Exception e )
            {
                logger.Error( e, "Error during background update check" );

                var backOff = GetNextBackOff();

                logger.Info( $"Backing off for {backOff} before retrying" );

#pragma warning disable 4014
                Task.Delay( backOff ).ContinueWith( _ => RunCheckAndQueueNextIfRequired() );
#pragma warning restore 4014
            }
        }

        private TimeSpan GetNextBackOff()
        {
            lastBackOff = TimeSpan.FromMilliseconds( lastBackOff.TotalMilliseconds * 2 );

            if ( lastBackOff > maxBackOff )
            {
                lastBackOff = maxBackOff;
            }

            return lastBackOff;
        }

        private void ChangeState( UpdateState state )
        {
            State = state;
            StateChanged?.Invoke( this, EventArgs.Empty );
        }
    }
}
