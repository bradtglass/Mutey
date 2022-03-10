namespace Mutey.Popup
{
    using System.Windows;
    using Mutey.Core.Audio;

    public partial class MicStatePopupControl
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
                                                                                              "State",
                                                                                              typeof( MuteState ),
                                                                                              typeof( MicStatePopupControl ),
                                                                                              new PropertyMetadata( default( MuteState ) ) );

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
                                                                                             "Size",
                                                                                             typeof( int ),
                                                                                             typeof( MicStatePopupControl ),
                                                                                             new PropertyMetadata( default( int ), OnSizeChanged ) );

        public MuteState State
        {
            get => (MuteState) GetValue( StateProperty );
            set => SetValue( StateProperty, value );
        }

        public int Size
        {
            get => (int) GetValue( SizeProperty );
            set => SetValue( SizeProperty, value );
        }

        public MicStatePopupControl()
        {
            InitializeComponent();
        }

        private static void OnSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            var control = (MicStatePopupControl) d;
            var size = (int) e.NewValue;

            using var _ = control.Dispatcher.DisableProcessing();
            control.Width = size;
            control.Height = size;
            control.ContainerBorder.CornerRadius = new CornerRadius( (double) size / 2 );
            control.Image.Margin = new Thickness( (double) size / 5 );
        }
    }
}
