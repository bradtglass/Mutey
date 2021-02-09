using System.ComponentModel;

namespace Mutey.Views
{
    public partial class CompactView
    {
        public CompactView()
        {
            Closing += OnClosing;

            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}