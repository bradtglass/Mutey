using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using Squirrel;

namespace Mutey
{
    [UsedImplicitly]
    public class UpdateService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan delayBetweenNonErroredChecked = TimeSpan.FromHours(1);
        private readonly TimeSpan maxBackOff = TimeSpan.FromMinutes(5);

        private readonly SemaphoreSlim updateSemaphore = new(1);
        private TimeSpan lastBackOff = TimeSpan.FromSeconds(2);


        public UpdateService()
        {
            State = UpdateState.NotChecked;
            Task.Run(RunCheckAndQueueNextIfRequired);
        }

        public UpdateState State { get; private set; }

        public event EventHandler? StateChanged;

        private async void RunCheckAndQueueNextIfRequired()
        {
            try
            {
                UpdateState refreshedState = await RefreshStateAsync();

                switch (refreshedState)
                {
                    case UpdateState.Latest:
                        // We're currently running the latest version, queue a check for later to check again
                        logger.Info(
                            $"Update check determined latest is installed, waiting {delayBetweenNonErroredChecked} for the next check");

#pragma warning disable 4014
                        Task.Delay(delayBetweenNonErroredChecked).ContinueWith(_ => RunCheckAndQueueNextIfRequired());
#pragma warning restore 4014
                        break;
                    case UpdateState.Available:
                        // No need to carry on checking because the update will be available for as long as the application is open
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error during background update check");

                TimeSpan backOff = GetNextBackOff();

                logger.Info($"Backing off for {backOff} before retrying");

#pragma warning disable 4014
                Task.Delay(backOff).ContinueWith(_ => RunCheckAndQueueNextIfRequired());
#pragma warning restore 4014
            }
        }

        private TimeSpan GetNextBackOff()
        {
            lastBackOff = TimeSpan.FromMilliseconds(lastBackOff.TotalMilliseconds * 2);

            if (lastBackOff > maxBackOff)
                lastBackOff = maxBackOff;

            return lastBackOff;
        }

        private void ChangeState(UpdateState state)
        {
            State = state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<UpdateState> RefreshStateAsync()
        {
            if (State == UpdateState.Available)
            {
                logger.Info("Skipping update check because state is already available");

                return UpdateState.Available;
            }

            await updateSemaphore.WaitAsync();
            try
            {
                logger.Info("Beginning update check");
                ChangeState(UpdateState.Checking);

                using IUpdateManager updateManager = await GetUpdateManagerAsync();
                UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                UpdateState newState;
                if (updateInfo.CurrentlyInstalledVersion.Version < updateInfo.FutureReleaseEntry.Version)
                {
                    logger.Info("Update check determined latest version is current version");
                    newState = UpdateState.Latest;
                }
                else
                {
                    logger.Info("Update check detected a newer version {Version}",
                        updateInfo.FutureReleaseEntry.Version);
                    newState = UpdateState.Available;
                }

                ChangeState(newState);

                return newState;
            }
            catch
            {
                ChangeState(UpdateState.NotChecked);
                throw;
            }
            finally
            {
                updateSemaphore.Release();
            }
        }

        private async Task<IUpdateManager> GetUpdateManagerAsync()
        {
            logger.Debug("Creating new update manager instance");

            return await UpdateManager.GitHubUpdateManager(@"https://github.com/G18SSY/Mutey", prerelease: true);
        }


        public async Task UpdateAsync()
        {
            await updateSemaphore.WaitAsync();
            try
            {
                using IUpdateManager updateManager = await GetUpdateManagerAsync();

                await updateManager.UpdateApp();
                logger.Info("Update applied");

                MessageBoxResult result = MessageBox.Show(
                    "Update applied successfully, would you like to restart to start using the latest version?",
                    "Update", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    logger.Info("Restarting application");
                    UpdateManager.RestartApp();
                }
            }
            finally
            {
                updateSemaphore.Dispose();
            }
        }
    }
}