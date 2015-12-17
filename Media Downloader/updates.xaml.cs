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
using System.IO.Compression;
using System.Net;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class updates : UserControl
    {
        public updates()
        {
            InitializeComponent();
            Loaded += update_Loaded;
        }

        //I don't feel like placing comments in this file yet.
        //Will do it someday later.
        //I also need to clean up this section, probably.

        WebClient WebClient = new WebClient();
        public object YouTubeDLPath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\youtube-dl.exe");
        public object DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
        public object RipMePath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\ripme.jar");
        public string ffmpegPath;
        private void update_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(YouTubeDLPath.ToString()))
            {
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + "youtube-dl.exe not found";
                string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
                UpdateYouTubeDL.IsEnabled = true;
                YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
            }
            else{
           
            Directory.SetCurrentDirectory(DownloadsFolder.ToString());
          
            //Here I get the current version of youtube-dl.exe, to get the version number, we have to run youtube-dl.exe --version
            Process youtubedl = new Process();
            youtubedl.StartInfo.CreateNoWindow = true;
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.StartInfo.RedirectStandardError = true;
            youtubedl.StartInfo.FileName = DownloadsFolder.ToString() + "\\youtube-dl.exe";
            youtubedl.StartInfo.Arguments = " --version";
            youtubedl.Start();
            string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
            CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;

            //We also need to get the latest version, so we can compare the latest version with the current version
            string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
            LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;

            int YouTubeDLUptodate = CurrentYouTubeDLVersion.CompareTo(LatestYoutubeDLVersion);

           // MessageBox.Show(YouTubeDLUptodate.ToString());
            // YouTubeDLUptodate = 1 means that the version is up to date
            // YouTubeDLUptodate = -1 means that the version is out of date
            if (YouTubeDLUptodate < 1)
            {
                YouTubeDLVersionStatusText.Text = "Your youtube-dl.exe is out of date, please click the button below to update.";
                  UpdateYouTubeDL.IsEnabled = true;
            }
            else
            {
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe is up to date!";
            }
            }

            if (!File.Exists(RipMePath.ToString()))
                {
                   
                    CurrentRipMeVersionText.Text = "Current RipMe version: " + "ripme.jar not found";
                    WebClient.DownloadFile("http://www.rarchives.com/ripme.json", DownloadsFolder + "\\latestripme.txt");
                    string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
                    File.Delete("latestripme.txt");
                    char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
                    LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
                    LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
                    UpdateRipMe.IsEnabled = true;
                    RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
                }
                else { 
                    Directory.SetCurrentDirectory(DownloadsFolder.ToString());
                Process ripme = new Process();
                ripme.StartInfo.CreateNoWindow = true;
                ripme.StartInfo.UseShellExecute = false;
                ripme.StartInfo.RedirectStandardOutput = true;
                ripme.StartInfo.RedirectStandardError = true;
                ripme.StartInfo.FileName = "java";
                ripme.StartInfo.Arguments = " -jar \"" + DownloadsFolder.ToString() + "\\ripme.jar\" --help";
                ripme.Start();

                //Getting the current RipMe version
                File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
                string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
                File.Delete("currentripme.txt");
                char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
                CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
                CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;

                //Getting the latest RipMe Version
                WebClient.DownloadFile("http://www.rarchives.com/ripme.json", DownloadsFolder + "\\latestripme.txt");
                string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
                File.Delete("latestripme.txt");
                char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
                LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
                LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;

                int RipMeUptodate = CurrentRipMeVersion.CompareTo(LatestRipMeVersion);
                if (RipMeUptodate < 0)
                {
                    RipMeVersionStatusText.Text = "Your RipMe is out of date, please click the button below to update.";
                    UpdateRipMe.IsEnabled = true;
                }
                else
                {
                    RipMeVersionStatusText.Text = "RipMe is up to date!";
                }
            }
            if (!File.Exists("C:\\ffmpeg\\bin\\ffmpeg.exe"))
            {
                MessageBox.Show("FFmpeg not found!");
            }
        
        }

        private void UpdateYouTubeDL_Click(object sender, RoutedEventArgs e)
        { 
            WebClient.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", DownloadsFolder + "\\youtube-dl.exe");
            Directory.SetCurrentDirectory(DownloadsFolder.ToString());
            Process youtubedl = new Process();
            youtubedl.StartInfo.CreateNoWindow = true;
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.StartInfo.RedirectStandardError = true;
            youtubedl.StartInfo.FileName = DownloadsFolder.ToString() + "\\youtube-dl.exe";
            youtubedl.StartInfo.Arguments = " --version";
            youtubedl.Start();
            string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
            CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
            UpdateYouTubeDL.IsEnabled = false;
            YouTubeDLVersionStatusText.Text = "Updated youtube-dl.exe";
        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            WebClient.DownloadFile("http://rarchives.com/ripme.jar", DownloadsFolder + "\\ripme.jar");
            Directory.SetCurrentDirectory(DownloadsFolder.ToString());
            Process ripme = new Process();
            ripme.StartInfo.CreateNoWindow = true;
            ripme.StartInfo.UseShellExecute = false;
            ripme.StartInfo.RedirectStandardOutput = true;
            ripme.StartInfo.RedirectStandardError = true;
            ripme.StartInfo.FileName = "java";
            ripme.StartInfo.Arguments = " -jar \"" + DownloadsFolder.ToString() + "\\ripme.jar\" --help";
            ripme.Start();
            File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
            string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
            File.Delete("currentripme.txt");
            char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
            CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
            CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
            UpdateRipMe.IsEnabled = false;
            RipMeVersionStatusText.Text = "Updated RipMe";
        }

        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            WebClient.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", DownloadsFolder + "\\rtmpdump.zip");
            //Damn, I need to extarct this .zip file, but I have no idea how!
            //For RipMe I need to extract an 7zip file, even harder, probably, need to sort that out some time
        }
    }
}
