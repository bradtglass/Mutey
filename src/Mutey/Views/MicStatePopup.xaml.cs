using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using NLog;
using Prism.Mvvm;

namespace Mutey.Views
{
    public partial class MicStatePopup
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private bool isDragging;
        private bool isPrimaryMouseDown;
        private Point mouseDownPosition;

        private MicStatePopup(Controller controller)
        {
            DataContext = controller;

            InitializeComponent();
        }

        public static void Flash(MuteState state)
        {
            Controller controller = new(state);

            MicStatePopup popup = new(controller);
            popup.Show();

            controller.IsVisible = true;
            Timer _ = new(p => EndPopup((MicStatePopup) p), popup, TimeSpan.FromSeconds(3),
                Timeout.InfiniteTimeSpan);
        }

        private static void EndPopup(MicStatePopup popup)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Controller controller = (Controller) popup.DataContext;
                controller.IsVisible = false;
            });

            Thread.Sleep(TimeSpan.FromSeconds(5));

            Application.Current.Dispatcher.Invoke(popup.Close);
        }

        private void MicStatePopup_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            isPrimaryMouseDown = true;
            isDragging = false;
            mouseDownPosition = e.GetPosition(this);
        }

        private void MicStatePopup_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            isPrimaryMouseDown = false;
            if (!isDragging)
                Close();
        }

        private void MicStatePopup_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isPrimaryMouseDown || isDragging)
                return;

            Point position = e.GetPosition(this);
            if (Math.Abs(position.X - mouseDownPosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - mouseDownPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                isDragging = true;
                try
                {
                    DragMove();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error activating DragMove");
                }
            }
        }


        private class Controller : BindableBase
        {
            private bool isVisible;


            private MuteState state;

            public Controller(MuteState state)
            {
                this.state = state;
            }

            public MuteState State
            {
                get => state;
                set => SetProperty(ref state, value);
            }

            public bool IsVisible
            {
                get => isVisible;
                set => SetProperty(ref isVisible, value);
            }
        }
    }
}