namespace Mutey.Hue
{
    using System;
    using System.Windows;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnExit( ExitEventArgs e )
        {
            ( Resources[ "AppViewModel" ] as IDisposable )?.Dispose();

            base.OnExit( e );
        }
    }
}
