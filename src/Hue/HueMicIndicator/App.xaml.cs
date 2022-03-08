using System;
using System.Windows;

namespace HueMicIndicator
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnExit(ExitEventArgs e)
        {
            (Resources["AppViewModel"] as IDisposable)?.Dispose();

            base.OnExit(e);
        }
    }
}