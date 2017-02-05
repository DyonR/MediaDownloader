using System.Diagnostics;
using System.Windows;
using System.Net;
using Microsoft.Win32;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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
                    using (client.OpenRead("https://www.google.com"))
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
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + "\\Media Downloads\\");
        }
    }
}
