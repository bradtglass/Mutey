using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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

            Loaded += OnLoaded;

            InitializeComponent();

            MoveToTopLeft();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Taken from https://stackoverflow.com/a/56648366/9766035
            // Variable to hold the handle for the form
            IntPtr helper = new WindowInteropHelper(this).Handle;
            // Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE,
                (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }

        #region ToolWindow

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // ReSharper disable InconsistentNaming
        // ReSharper disable IdentifierTypo
        private const int GWL_EX_STYLE = -20;

        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        // ReSharper restore InconsistentNaming
        // ReSharper restore IdentifierTypo

        #endregion

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