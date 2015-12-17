using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Net;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for supported.xaml
    /// </summary>
    public partial class supported : UserControl
    {
        public supported()
        {
            InitializeComponent();
        }

        public object DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
        private void youtubedlButton_Checked(object sender, RoutedEventArgs e)
        {
            Process youtubedl = new Process();
            youtubedl.StartInfo.CreateNoWindow = true;
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.StartInfo.RedirectStandardError = true;
            youtubedl.StartInfo.FileName = DownloadsFolder.ToString() + "\\youtube-dl.exe";
            youtubedl.StartInfo.Arguments = " --list-extractors";
            youtubedl.Start();
            supportedTextBox.Text = youtubedl.StandardOutput.ReadToEnd();
        }

        private void ripmeButton_Checked(object sender, RoutedEventArgs e)
        {
            supportedTextBox.Text = "4chan and other *chans\r\n500px\r\n8muses\r\nanonib\r\nbcfakes\r\nbutttoucher.com\r\ncheeby\r\ndatw.in\r\ndeviantart\r\ndrawcrowd\r\nfapproved\r\nflickr\r\nfuraffinity\r\nfuskator\r\ngifyo\r\n\"gonewild\"\r\nhentai-foundry\r\ni.rarchives.com\r\nimagebam\r\nimagestash.org\r\nimagevenue\r\nimgbox.com\r\nimgur (album, userpages, subreddits)\r\ninstagram\r\nkinkyshare.com\r\nmediacru.sh\r\nminus\r\nmodelmayhem\r\nmotherless\r\nnfsfw\r\nphotobucket\r\npornhub\r\nreddit\r\nseenive\r\nsmutty.com\r\nsupertangas\r\ntaptastic\r\nteenplanet\r\ntumblr\r\ntwitter\r\nvine\r\nvinebox\r\nvk.com (images, albums, video, and video albums)\r\nxhamster (images)";
        }
    }
}
