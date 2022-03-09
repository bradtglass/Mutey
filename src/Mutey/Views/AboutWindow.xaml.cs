using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Navigation;
using NLog;

namespace Mutey.Views
{
    public partial class AboutWindow
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public AboutWindow()
        {
            InitializeComponent();
        }

        public string Version
        {
            get
            {
                Assembly assembly = GetType().Assembly;

                return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
                       assembly.GetName().Version!.ToString();
            }
        }

        public string RuntimeInfo
            => RuntimeInformation.FrameworkDescription;

        public string OsInfo
            => RuntimeInformation.OSDescription;
        
        private void GithubMouseDown(object sender, MouseButtonEventArgs e)
            => Navigate(new Uri(@"https://github.com/G18SSY/Mutey"));

        private static void Navigate(Uri uri)
        {
            try
            {
                ProcessStartInfo startInfo = new(uri.ToString())
                {
                    UseShellExecute = true
                };
                
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to navigate to {Uri}", uri);
            }
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
            => Navigate(e.Uri);
    }
}