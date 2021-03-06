﻿using System.Windows;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for supported.xaml
    /// </summary>
    public partial class supported
    {
        public supported()
        {
            InitializeComponent();
        }

        BackgroundWorker youtubedlSupported;

        private void youtubedlButton_Checked(object sender, RoutedEventArgs e)
        {
            youtubedlSupported = new BackgroundWorker {WorkerReportsProgress = true};
            youtubedlSupported.DoWork += (obj, ea) => youtubedlSupported_Process();
            youtubedlSupported.RunWorkerAsync();
        }

        public void youtubedlSupported_Process()
        {
            if (File.Exists(youtubedl.YouTubeDLPath))
            {
                var youtubedl = new Process
                {
                    StartInfo =
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = MediaDownloader.youtubedl.YouTubeDLPath,
                        Arguments = " --list-extractors"
                    }
                };
                youtubedl.Start();
                Dispatcher.Invoke(() =>
                {
                    supportedTextBox.Text = youtubedl.StandardOutput.ReadToEnd();
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    supportedTextBox.Text = @"youtube-dl not found.
Please go to the Updates tab, and download youtube-dl.";
                });
            }
        }

        //Too bad, RipMe does not have a command to print all supported URLs, thus we need to manually enter it.
        private void ripmeButton_Checked(object sender, RoutedEventArgs e)
        {
            supportedTextBox.Text = "4chan and other *chans\r\n8muses\r\n500px\r\n\"gonewild\"\r\nanonib\r\nbcfakes\r\nbutttoucher.com\r\ncheeby\r\ndatw.in\r\ndev\r\nantart\r\ndrawcrowd\r\nfapproved\r\nflickr\r\nfuraffinity\r\nfuskator\r\ngifyo\r\nhenta\r\ni.rarchives.com\r\nimagebam\r\nimagestash.org\r\nimagevenue\r\nimgbox.com\r\nimgur (album, userpages, subreddits)\r\ninstagram\r\nkinkyshare.com\r\nmediacru.sh\r\nminus\r\nmodelmayhem\r\nmotherless\r\nnfsfw\r\np\r\notobucket\r\npornhub\r\nreddit\r\nseenive\r\nsmutty.com\r\nsupertangas\r\ntaptastic\r\nteenplanet\r\ntumblr\r\ntwitter\r\nvine\r\nvinebox\r\nvk.com (images, albums, video, and video albums)\r\nxhamster (images)";
        }
    }
}
