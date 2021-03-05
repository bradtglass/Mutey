using System.Windows;
using System.Windows.Threading;

namespace Mutey.Popup
{
    public partial class MicStatePopupControl
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(MuteState),
            typeof(MicStatePopupControl),
            new PropertyMetadata(default(MuteState)));

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size",
            typeof(int),
            typeof(MicStatePopupControl),
            new PropertyMetadata(default(int), OnSizeChanged));

        public MicStatePopupControl()
        {
            InitializeComponent();
        }

        public MuteState State
        {
            get => (MuteState) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public int Size
        {
            get => (int) GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MicStatePopupControl control = (MicStatePopupControl) d;
            int size = (int) e.NewValue;

            Size original = new(control.ActualWidth, control.ActualHeight);
            using DispatcherProcessingDisabled _ = control.Dispatcher.DisableProcessing();
            control.Width = size;
            control.Height = size;
            control.ContainerBorder.CornerRadius = new CornerRadius((double) size / 2);
            control.Image.Margin = new Thickness((double) size / 5);

            // control.OnRenderSizeChanged(new SizeChangedInfo(control, original, true, true));
        }
    }
}