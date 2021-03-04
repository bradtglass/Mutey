using System;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace Mutey.Popup
{
    internal partial class MicStatePopup
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly MicStatePopupViewModel controller;

        public MicStatePopup(MicStatePopupViewModel controller)
        {
            this.controller = controller;
            DataContext = controller;

            InitializeComponent();

            MoveToTopLeft();
        }

        private void MoveToTopLeft()
        {
            const int margin = 40;

            Point topRight = SystemParameters.WorkArea.TopRight;
            Left = topRight.X - Width - margin;
            Top = topRight.Y + margin;
        }

        #region Mouse Handling

        private bool isDragging;
        private bool isPrimaryMouseDown;
        private Point mouseDownPosition;

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
                controller.IsVisible = false;
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

        #endregion
    }
}