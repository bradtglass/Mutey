using System.ComponentModel;
using System.Windows;

namespace Mutey.Views
{
    public partial class CompactView : Window
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