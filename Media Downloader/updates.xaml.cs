using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class updates
    {
        public updates()
        {
            InitializeComponent();
            Loaded += update_Loaded;
        }
        public string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + (@"\Media Downloads");
        public string LocalStorageFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\");
        public string YouTubeDLPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\youtube-dl.exe");
        public string RipMePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ripme.jar");
        public string ffmpegfolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ffmpeg");
        public string RTMPDumpPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\rtmpdump.exe");

        public string LatestYoutubeDLVersion;
        public string CurrentYouTubeDLVersion;
        public string LatestFFmpegVersion;
        public string CurrentFFmpegVersion;
        public string LatestRipMeVersion;
        public string CurrentRipMeVersion;

        public bool checkforUpdates = false;
        public bool youtubedlUpdating = false;
        public bool ripmeUpdating = false;
        public bool ffmpegUpdating = false;
        public bool rtmpdumpUpdating = false;

        BackgroundWorker youtubedlWorker;
        BackgroundWorker youtubedlUpdateWorker;
        BackgroundWorker ripmeWorker;
        BackgroundWorker ripmeUpdateWorker;
        BackgroundWorker ffmpegWorker;
        BackgroundWorker ffmpegUpdateWorker;
        BackgroundWorker RTMPDumpWorker;
        BackgroundWorker RTMPDumpUpdateWorker;

        WebClient Clientyoutubedl = new WebClient();
        WebClient ClientRipMe = new WebClient();
        WebClient ClientFFmpeg = new WebClient();
        WebClient ClientRTMPDump = new WebClient();

        private void update_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= update_Loaded;

            youtubedlWorker = new BackgroundWorker();
            youtubedlWorker.WorkerReportsProgress = true;
            youtubedlWorker.DoWork += (obj, ea) => youtubedlGetCurrentVersion_Process();
            youtubedlWorker.RunWorkerAsync();

            ffmpegWorker = new BackgroundWorker();
            ffmpegWorker.WorkerReportsProgress = true;
            ffmpegWorker.DoWork += (obj, ea) => ffmpegGetCurrentVersion_Process();
            ffmpegWorker.RunWorkerAsync();

            ripmeWorker = new BackgroundWorker();
            ripmeWorker.WorkerReportsProgress = true;
            ripmeWorker.DoWork += (obj, ea) => RipMeGetCurrentVersion_Process();
            ripmeWorker.RunWorkerAsync();

            RTMPDumpWorker = new BackgroundWorker();
            RTMPDumpWorker.WorkerReportsProgress = true;
            RTMPDumpWorker.DoWork += (obj, ea) => RTMPDumpCompareVersion_Process();
            RTMPDumpWorker.RunWorkerAsync();
        }
        private void UpdateYouTubeDL_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                youtubedlUpdateWorker = new BackgroundWorker();
                youtubedlUpdateWorker.WorkerReportsProgress = true;
                youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlInstallLastestVersion_Process();
                youtubedlUpdateWorker.RunWorkerAsync(youtubedlUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                ripmeUpdateWorker = new BackgroundWorker();
                ripmeUpdateWorker.WorkerReportsProgress = true;
                ripmeUpdateWorker.DoWork += (obj, ea) => RipMeInstallLastestVersion_Process();
                ripmeUpdateWorker.RunWorkerAsync(ripmeUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                ffmpegUpdateWorker = new BackgroundWorker();
                ffmpegUpdateWorker.WorkerReportsProgress = true;
                ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegInstallLastestVersion_Process();
                ffmpegUpdateWorker.RunWorkerAsync(ffmpegUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                RTMPDumpUpdateWorker = new BackgroundWorker();
                RTMPDumpUpdateWorker.WorkerReportsProgress = true;
                RTMPDumpUpdateWorker.DoWork += (obj, ea) => RTMPDumpInstallLastestVersion_Process();
                RTMPDumpUpdateWorker.RunWorkerAsync(rtmpdumpUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void youtubedlGetCurrentVersion_Process()
        {
            if (File.Exists(YouTubeDLPath))
            {
                //Here I get the current version of youtube-dl.exe, to get the version number, we have to run youtube-dl.exe --version
                Process youtubedl = new Process();
                youtubedl.StartInfo.CreateNoWindow = true;
                youtubedl.StartInfo.UseShellExecute = false;
                youtubedl.StartInfo.RedirectStandardOutput = true;
                youtubedl.StartInfo.RedirectStandardError = true;
                youtubedl.StartInfo.FileName = YouTubeDLPath;
                youtubedl.StartInfo.Arguments = " --version";
                youtubedl.Start();
                CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
                Dispatcher.Invoke(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
                    YouTubeDLVersionStatusText.Text = null;
                    UpdateYouTubeDL.IsEnabled = false;
                });

            }
            else {
                Dispatcher.Invoke(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: youtube-dl.exe not found.";
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
                    UpdateYouTubeDL.Content = "Update youtube-dl";
                    UpdateYouTubeDL.IsEnabled = true;
                });
            }
        }
        private void youtubedlGetLatestVersion_Process()
        {
            LatestYoutubeDLVersion = Clientyoutubedl.DownloadString("https://yt-dl.org/latest/version");
            Dispatcher.Invoke(() =>
            {
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
            });
        }
        private void youtubedlCompareVersion_Process()
        {
            youtubedlGetCurrentVersion_Process();
            youtubedlGetLatestVersion_Process();
            if (File.Exists(YouTubeDLPath))
            {
                int YouTubeDLUptodate = CurrentYouTubeDLVersion.CompareTo(LatestYoutubeDLVersion);
                if (YouTubeDLUptodate < 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "Your youtube-dl.exe is out of date, please click the button below to update.";
                        UpdateYouTubeDL.Content = "Update youtube-dl";
                        UpdateYouTubeDL.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "youtube-dl.exe is up to date!";
                        UpdateYouTubeDL.IsEnabled = false;
                    });
                }
            }
        }
        private void youtubedlInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                YouTubeDLVersionStatusText.Text = null;
                UpdateYouTubeDL.Content = "Getting latest version...";
                UpdateYouTubeDL.IsEnabled = false;
            });
            youtubedlGetLatestVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Downloading youtube-dl...";
            });
            Clientyoutubedl.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", YouTubeDLPath);
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Getting current version...";
            });
            youtubedlGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Update finished";
            });
            youtubedlUpdating = false;
        }
        private void RipMeGetCurrentVersion_Process()
        {
            if (File.Exists(RipMePath))
            {
                Directory.SetCurrentDirectory(LocalStorageFolder);
                Process ripme = new Process();
                ripme.StartInfo.CreateNoWindow = true;
                ripme.StartInfo.UseShellExecute = false;
                ripme.StartInfo.RedirectStandardOutput = true;
                ripme.StartInfo.RedirectStandardError = true;
                ripme.StartInfo.FileName = "java";
                ripme.StartInfo.Arguments = " -jar \"" + RipMePath + "\" --help";
                ripme.Start();

                //Getting the current RipMe version
                CurrentRipMeVersion = ripme.StandardOutput.ReadToEnd()
                    .Split(Environment.NewLine.ToCharArray())[4]
                    .Replace("Initialized ripme v", "");

                Dispatcher.Invoke(() =>
                {
                    CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
                    RipMeVersionStatusText.Text = null;
                    UpdateRipMe.IsEnabled = false;
                });
            }
            else
            {
                Dispatcher.Invoke(() => {
                    CurrentRipMeVersionText.Text = "Current RipMe version: ripme.jar not found";
                    RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
                    UpdateRipMe.Content = "Update RipMe";
                    UpdateRipMe.IsEnabled = true;
                });
            }
        }
        private void RipMeGetLatestVersion_Process()
        {
            JObject LatestRipMeJson = JObject.Parse(ClientRipMe.DownloadString("http://www.rarchives.com/ripme.json"));
            LatestRipMeVersion = (string)LatestRipMeJson["latestVersion"];
            Dispatcher.Invoke(() =>
           {
                 LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
           });
        }
        private void RipMeCompareVersion_Process()
        {
            RipMeGetCurrentVersion_Process();
            RipMeGetLatestVersion_Process();
            if (File.Exists(RipMePath))
            {
                //Higher version than newest = 1
                //Outdated = -1
                //Same version = 0
                int RipMeUptodate = CurrentRipMeVersion.CompareTo(LatestRipMeVersion);
                if (RipMeUptodate < 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RipMeVersionStatusText.Text = "Your RipMe is out of date, please click the button below to update.";
                        UpdateRipMe.Content = "Update RipMe";
                        UpdateRipMe.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        RipMeVersionStatusText.Text = "RipMe is up to date!";
                        UpdateRipMe.IsEnabled = false;
                    });
                }
            }
        }
        private void RipMeInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                RipMeVersionStatusText.Text = null;
                UpdateRipMe.Content = "Getting latest version...";
                UpdateRipMe.IsEnabled = false;
            });
            RipMeGetLatestVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Downloading RipMe...";
            });
            ClientRipMe.DownloadFile("http://rarchives.com/ripme.jar", RipMePath);
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Getting current version...";
            });
            RipMeGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Update finished";
            });
            ripmeUpdating = false;
        }
        private void ffmpegGetCurrentVersion_Process()
        {
            if (File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
            {
                CurrentFFmpegVersion = File.ReadLines(ffmpegfolderPath + "\\bin\\version.txt").First();
                Dispatcher.Invoke(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                    FFmpegVersionStatusText.Text = null;
                    UpdateFFmpeg.IsEnabled = false;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: FFmpeg not found.";
                    FFmpegVersionStatusText.Text = "ffmpeg can not be found, please click the button below.";
                    UpdateFFmpeg.Content = "Update FFmpeg";
                    UpdateFFmpeg.IsEnabled = true;
                });
            }
        }
        private void ffmpegGetLatestVersion_Process()
        {
            Process GetFFmpeg = new Process();
            GetFFmpeg.StartInfo.CreateNoWindow = true;
            GetFFmpeg.StartInfo.UseShellExecute = false;
            File.WriteAllText(LocalStorageFolder + "\\Getffmpeg.ps1",
            @"$DownloadsLocation = $env:LOCALAPPDATA + ""\Media Downloader""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 3 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
            GetFFmpeg.StartInfo.FileName = "powershell.exe";
            GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + LocalStorageFolder + "\\Getffmpeg.ps1\"";
            GetFFmpeg.Start();
            GetFFmpeg.WaitForExit();
            LatestFFmpegVersion = File.ReadLines(LocalStorageFolder + "\\LastFFmpeg.txt").First();
            Dispatcher.Invoke(() =>
            {
                LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
            });
            try
            {
                File.Delete(LocalStorageFolder + "\\LatestFFmpeg.txt");
                File.Delete(LocalStorageFolder + "\\LastFFmpeg.txt");
                File.Delete(LocalStorageFolder + "\\Getffmpeg.ps1");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to delete LatestFFmpeg.txt, LastFFmpeg.txt or Getffmpeg.ps1");
            }
        }
        private void ffmpegCompareVersion_Process()
        {
            ffmpegGetCurrentVersion_Process();
            ffmpegGetLatestVersion_Process();
            if (File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
            {
                int FFmpegUptodate = CurrentFFmpegVersion.CompareTo(LatestFFmpegVersion);
                //Higher version = 1
                //Outdated = -1 
                //Same version = 0
                if (FFmpegUptodate == -1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FFmpegVersionStatusText.Text = "Your FFmpeg is out of date, please click the button below to update.";
                        UpdateFFmpeg.Content = "Update FFmpeg";
                        UpdateFFmpeg.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        FFmpegVersionStatusText.Text = "FFmpeg is up to date!";
                        UpdateFFmpeg.IsEnabled = false;
                    });
                }
            }
        }
        private void ffmpegInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                FFmpegVersionStatusText.Text = null;
                UpdateFFmpeg.Content = "Getting latest version...";
                UpdateFFmpeg.IsEnabled = false;
            });
            ffmpegGetLatestVersion_Process();
            try
            {
                Directory.Delete(ffmpegfolderPath, true);
            }
            catch (Exception)
            {
                if (Directory.Exists(ffmpegfolderPath))
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine("Can not delete " + ffmpegfolderPath + ".");
                    errorMessage.AppendLine("If you never installed FFmpeg, you can ignore this.");
                    errorMessage.AppendLine("Updating might fail.");
                    MessageBox.Show(errorMessage.ToString());
                }
            }
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Downloading FFmpeg...";
            });
            ClientFFmpeg.DownloadFile("http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", LocalStorageFolder + "\\ffmpeg.zip");
            ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\ffmpeg.zip");
            foreach (ZipEntry file in zip)
            {
                file.Extract(LocalStorageFolder);
            }
            zip.Dispose();
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Moving files...";
            });
            Directory.Move(LocalStorageFolder + "\\ffmpeg-latest-win64-static", LocalStorageFolder + "\\ffmpeg");

            try { File.Delete(LocalStorageFolder + "UpdateFFmpeg.ps1"); }
            catch { MessageBox.Show("Unable to delete: " + LocalStorageFolder + "\\UpdateFFmpeg.ps1"); }

            try { File.Delete(LocalStorageFolder + "\\ffmpeg.zip"); }
            catch { MessageBox.Show("Unable to delete: " + LocalStorageFolder + "\\ffmpeg.zip"); }


            File.WriteAllText(ffmpegfolderPath + "\\bin\\version.txt", LatestFFmpegVersion);
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Getting current version...";
            });
            ffmpegGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Update finished";
            });
            ffmpegUpdating = false;
        }
        private void RTMPDumpInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                DownloadRTMPDump.Content = "Downloading RTMPdump..";
                DownloadRTMPDump.IsEnabled = false;
            });
            ClientRTMPDump.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", LocalStorageFolder + "\\rtmpdump.zip");
            ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\rtmpdump.zip");
            ZipEntry rtmpdumpZip = zip["rtmpdump.exe"];
            rtmpdumpZip.Extract(LocalStorageFolder);
            zip.Dispose();
            File.Delete(LocalStorageFolder + "\\rtmpdump.zip");
            youtubedlGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                DownloadRTMPDump.Content = "Update finished";
            });
            rtmpdumpUpdating = false;
        }
        private void RTMPDumpCompareVersion_Process()
        {
                if (!File.Exists(RTMPDumpPath))
                {
                Dispatcher.Invoke(() => {
                    DownloadRTMPDump.Content = "Download RTMPDump";
                    DownloadRTMPDump.IsEnabled = true;
                });
            }
        }
        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (youtubedlUpdating == false  && ripmeUpdating == false && ffmpegUpdating == false && rtmpdumpUpdating == false)
            {
                checkforUpdates = true;
                Dispatcher.Invoke(() =>
                {
                CheckForUpdates.IsEnabled = false;
                });
                youtubedlUpdateWorker = new BackgroundWorker();
                youtubedlUpdateWorker.WorkerReportsProgress = true;
                youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlCompareVersion_Process();
                youtubedlUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(youtubedlUpdateWorker_Process_Complete);
                youtubedlUpdateWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void youtubedlUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ripmeUpdateWorker = new BackgroundWorker();
            ripmeUpdateWorker.WorkerReportsProgress = true;
            ripmeUpdateWorker.DoWork += (obj, ea) => RipMeCompareVersion_Process();
            ripmeUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ripmeUpdateWorker_Process_Complete);
            ripmeUpdateWorker.RunWorkerAsync();
        }
        private void ripmeUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ffmpegUpdateWorker = new BackgroundWorker();
            ffmpegUpdateWorker.WorkerReportsProgress = true;
            ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegCompareVersion_Process();
            ffmpegUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ffmpegUpdateWorker_Process_Complete);
            ffmpegUpdateWorker.RunWorkerAsync();
        }

        private void ffmpegUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            RTMPDumpUpdateWorker = new BackgroundWorker();
            RTMPDumpUpdateWorker.WorkerReportsProgress = true;
            RTMPDumpUpdateWorker.DoWork += (obj, ea) => RTMPDumpCompareVersion_Process();
            RTMPDumpUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RTMPDumpUpdateWorker_Process_Complete);
            RTMPDumpUpdateWorker.RunWorkerAsync();
        }

        private void RTMPDumpUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckForUpdates.IsEnabled = true;
            checkforUpdates = false;
        }
    }
}
