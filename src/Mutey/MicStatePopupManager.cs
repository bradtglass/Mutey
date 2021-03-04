using System;
using System.Windows.Threading;
using Mutey.Views;

namespace Mutey
{
    public class MicStatePopupManager : IDisposable
    {
        private readonly MicStatePopup.Controller controller;
        private readonly MicStatePopup popup;

        public MicStatePopupManager()
        {
            controller = new MicStatePopup.Controller();
            popup = new MicStatePopup(controller);

            popup.Show();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public void Flash(MuteState state)
        {
            controller.State = state;
            controller.IsVisible = true;
            
            DispatcherTimer timer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += EndFlashTick;
            timer.Start();
        }

        private void EndFlashTick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer timer)
                timer.Stop();

            EndPopup();
        }

        private void EndPopup()
        {
            controller.IsVisible = false;
        }

        private void ReleaseUnmanagedResources()
        {
            popup.Close();
        }

        ~MicStatePopupManager()
        {
            ReleaseUnmanagedResources();
        }
    }
}