namespace Mutey.Popup
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Mutey.Core.Settings;
    using NLog;

    internal partial class MicStatePopup
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly MicStatePopupViewModel viewModel;

        public MicStatePopup( ISettingsStore settingsStore, MicStatePopupViewModel viewModel )
        {
            this.viewModel = viewModel;
            DataContext = viewModel;

            Loaded += OnLoaded;
            ContentRendered += OnContentRendered;
            InitializeComponent();

            settingsStore.RegisterForNotifications<MuteySettings>(SettingsChanged);
            SettingsChanged(settingsStore.Get<MuteySettings>());
        }

        private void SettingsChanged( SettingsChangedEventArgs<MuteySettings> args )
            => SettingsChanged( args.NewValue );
        
        private void SettingsChanged( MuteySettings settings )
        {
            StatePopupControl.Size = settings.MuteStatePopupSize;
        }
        
        private void OnContentRendered( object? sender, EventArgs e )
        {
            MoveToStartupPosition();
        }

        private void OnLoaded( object sender, RoutedEventArgs e )
        {
            // Taken from https://stackoverflow.com/a/56648366/9766035
            // Variable to hold the handle for the form
            var helper = new WindowInteropHelper( this ).Handle;
            // Performing some magic to hide the form from Alt+Tab
            SetWindowLong( helper, GWL_EX_STYLE,
                           ( GetWindowLong( helper, GWL_EX_STYLE ) | WS_EX_TOOLWINDOW ) & ~WS_EX_APPWINDOW );
        }

        private void MoveToStartupPosition()
        {
            const int margin = 40;

            var topRight = SystemParameters.WorkArea.TopRight;
            Left = topRight.X - Width - margin;
            Top = topRight.Y + margin;
        }

        private void MuteStateControl_OnSizeChanged( object sender, SizeChangedEventArgs e )
        {
            using var _ = Dispatcher.DisableProcessing();

            // Store current centre
            double xMiddle = Left + ActualWidth * 0.5;
            double yMiddle = Top + ActualHeight * 0.5;

            // Resize to content
            // (Assuming no margin here)
            var control = (Control) sender;
            Height = control.ActualHeight;
            Width = control.ActualWidth;

            // Move window to keep centre in same position instead of top left
            Left = xMiddle - ActualWidth * 0.5;
            Top = yMiddle - ActualHeight * 0.5;
        }

        #region ToolWindow

        [ DllImport( "user32.dll", SetLastError = true ) ]
        private static extern int GetWindowLong( IntPtr hWnd, int nIndex );

        [ DllImport( "user32.dll" ) ]
        private static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );

        // ReSharper disable InconsistentNaming
        // ReSharper disable IdentifierTypo
        private const int GWL_EX_STYLE = -20;

        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        // ReSharper restore InconsistentNaming
        // ReSharper restore IdentifierTypo

        #endregion

        #region Mouse Handling

        private bool isDragging;
        private bool isPrimaryMouseDown;
        private Point mouseDownPosition;

        private void MicStatePopup_OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( e.ChangedButton != MouseButton.Left )
            {
                return;
            }

            isPrimaryMouseDown = true;
            isDragging = false;
            mouseDownPosition = e.GetPosition( this );
        }

        private void MicStatePopup_OnMouseUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ChangedButton != MouseButton.Left )
            {
                return;
            }

            isPrimaryMouseDown = false;
            if ( !isDragging && viewModel.PopupPressedCommand.CanExecute( null ) )
            {
                viewModel.PopupPressedCommand.Execute( null );
            }
        }

        private void MicStatePopup_OnMouseMove( object sender, MouseEventArgs e )
        {
            if ( !isPrimaryMouseDown || isDragging )
            {
                return;
            }

            var position = e.GetPosition( this );
            if ( Math.Abs( position.X - mouseDownPosition.X ) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs( position.Y - mouseDownPosition.Y ) > SystemParameters.MinimumVerticalDragDistance )
            {
                isDragging = true;
                try
                {
                    DragMove();
                }
                catch ( Exception ex )
                {
                    logger.Error( ex, "Error activating DragMove" );
                }
            }
        }

        #endregion
    }
}
