using System.Windows;
using MahApps.Metro.Controls;
using System.Net;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            CheckForInternetConnection();
            InitializeComponent();
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Media Downloader requires a working internet connection.
Either your internet is not working or Google.com is unreachable.", "No internet connection ", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return false;
            }
        }
    }
}
