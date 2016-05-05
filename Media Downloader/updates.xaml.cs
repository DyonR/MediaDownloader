using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.ComponentModel;
using System.Text;

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

        public int checkforUpdates = 0;
        public int youtubedlUpdating = 0;
        public int ripmeUpdating = 0;
        public int ffmpegUpdating = 0;
        public int rtmpdumpUpdating = 0; 

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
        WebClient Client7zip = new WebClient();
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
            if (checkforUpdates != 1)
            {
                youtubedlUpdateWorker = new BackgroundWorker();
                youtubedlUpdateWorker.WorkerReportsProgress = true;
                youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlInstallLastestVersion_Process();
                youtubedlUpdateWorker.RunWorkerAsync(youtubedlUpdating = 1);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates != 1)
            {
                ripmeUpdateWorker = new BackgroundWorker();
                ripmeUpdateWorker.WorkerReportsProgress = true;
                ripmeUpdateWorker.DoWork += (obj, ea) => RipMeInstallLastestVersion_Process();
                ripmeUpdateWorker.RunWorkerAsync(ripmeUpdating = 1);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates != 1)
            {
                ffmpegUpdateWorker = new BackgroundWorker();
                ffmpegUpdateWorker.WorkerReportsProgress = true;
                ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegInstallLastestVersion_Process();
                ffmpegUpdateWorker.RunWorkerAsync(ffmpegUpdating = 1);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates != 1)
            {
                RTMPDumpUpdateWorker = new BackgroundWorker();
                RTMPDumpUpdateWorker.WorkerReportsProgress = true;
                RTMPDumpUpdateWorker.DoWork += (obj, ea) => RTMPDumpInstallLastestVersion_Process();
                RTMPDumpUpdateWorker.RunWorkerAsync(rtmpdumpUpdating = 1);
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
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
                    YouTubeDLVersionStatusText.Text = null;
                    UpdateYouTubeDL.IsEnabled = false;
                }));

            }
            else {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: youtube-dl.exe not found.";
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
                    UpdateYouTubeDL.Content = "Update youtube-dl";
                    UpdateYouTubeDL.IsEnabled = true;
                }));
            }
        }
        private void youtubedlGetLatestVersion_Process()
        {
            LatestYoutubeDLVersion = Clientyoutubedl.DownloadString("https://yt-dl.org/latest/version");
            this.Dispatcher.Invoke((Action)(() =>
            {
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
            }));
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
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "Your youtube-dl.exe is out of date, please click the button below to update.";
                        UpdateYouTubeDL.Content = "Update youtube-dl";
                        UpdateYouTubeDL.IsEnabled = true;
                    }));
                }
                else
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "youtube-dl.exe is up to date!";
                        UpdateYouTubeDL.IsEnabled = false;
                    }));
                }
            }
        }
        private void youtubedlInstallLastestVersion_Process()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                YouTubeDLVersionStatusText.Text = null;
                UpdateYouTubeDL.Content = "Getting latest version...";
                UpdateYouTubeDL.IsEnabled = false;
            }));
            youtubedlGetLatestVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateYouTubeDL.Content = "Downloading youtube-dl...";
            }));
            Clientyoutubedl.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", YouTubeDLPath);
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateYouTubeDL.Content = "Getting current version...";
            }));
            youtubedlGetCurrentVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateYouTubeDL.Content = "Update finished";
            }));
            youtubedlUpdating = 0;
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
                File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
                CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
                File.Delete("currentripme.txt");
                char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
                CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
                    RipMeVersionStatusText.Text = null;
                    UpdateRipMe.IsEnabled = false;
                }));
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() => {
                    CurrentRipMeVersionText.Text = "Current RipMe version: ripme.jar not found";
                    RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
                    UpdateRipMe.Content = "Update RipMe";
                    UpdateRipMe.IsEnabled = true;
                }));
            }
        }
        private void RipMeGetLatestVersion_Process()
        {
            ClientRipMe.DownloadFile("http://www.rarchives.com/ripme.json", LocalStorageFolder + "\\latestripme.txt");
            LatestRipMeVersion = File.ReadLines(LocalStorageFolder + "\\latestripme.txt").Skip(1).First();
            File.Delete(LocalStorageFolder + "\\latestripme.txt");
            char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
            LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
           this.Dispatcher.Invoke((Action)(() =>
           {
               LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
        }));
        }
        private void RipMeCompareVersion_Process()
        {
            RipMeGetCurrentVersion_Process();
            RipMeGetLatestVersion_Process();
            if (File.Exists(RipMePath))
            {
                int RipMeUptodate = CurrentRipMeVersion.CompareTo(LatestRipMeVersion);
                if (RipMeUptodate < 0)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        RipMeVersionStatusText.Text = "Your RipMe is out of date, please click the button below to update.";
                        UpdateRipMe.Content = "Update RipMe";
                        UpdateRipMe.IsEnabled = true;
                    }));
                }
                else
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        RipMeVersionStatusText.Text = "RipMe is up to date!";
                        UpdateRipMe.IsEnabled = false;
                    }));
                }
            }
        }
        private void RipMeInstallLastestVersion_Process()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                RipMeVersionStatusText.Text = null;
                UpdateRipMe.Content = "Getting latest version...";
                UpdateRipMe.IsEnabled = false;
            }));
            RipMeGetLatestVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateRipMe.Content = "Downloading RipMe...";
            }));
            ClientRipMe.DownloadFile("http://rarchives.com/ripme.jar", RipMePath);
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateRipMe.Content = "Getting current version...";
            }));
            RipMeGetCurrentVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateRipMe.Content = "Update finished";
            }));
            ripmeUpdating = 0;
        }
        private void ffmpegGetCurrentVersion_Process()
        {
            if (File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
            {
                CurrentFFmpegVersion = File.ReadLines(ffmpegfolderPath + "\\bin\\version.txt").First();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                    FFmpegVersionStatusText.Text = null;
                    UpdateFFmpeg.IsEnabled = false;
                }));
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: FFmpeg not found.";
                    FFmpegVersionStatusText.Text = "ffmpeg can not be found, please click the button below.";
                    UpdateFFmpeg.Content = "Update FFmpeg";
                    UpdateFFmpeg.IsEnabled = true;
                }));
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
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
            GetFFmpeg.StartInfo.FileName = "powershell.exe";
            GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + LocalStorageFolder + "\\Getffmpeg.ps1\"";
            GetFFmpeg.Start();
            GetFFmpeg.WaitForExit();
            LatestFFmpegVersion = File.ReadLines(LocalStorageFolder + "\\LastFFmpeg.txt").First();
            this.Dispatcher.Invoke((Action)(() =>
            {
                LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
            }));
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
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        FFmpegVersionStatusText.Text = "Your FFmpeg is out of date, please click the button below to update.";
                        UpdateFFmpeg.Content = "Update FFmpeg";
                        UpdateFFmpeg.IsEnabled = true;
                    }));
                }
                else
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        FFmpegVersionStatusText.Text = "FFmpeg is up to date!";
                        UpdateFFmpeg.IsEnabled = false;
                    }));
                }
            }
        }
        private void ffmpegInstallLastestVersion_Process()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                FFmpegVersionStatusText.Text = null;
                UpdateFFmpeg.Content = "Getting latest version...";
                UpdateFFmpeg.IsEnabled = false;
            }));
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
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateFFmpeg.Content = "Downloading 7-zip...";
            }));
            Client7zip.DownloadFile("http://www.7-zip.org/a/7za920.zip", LocalStorageFolder + "\\7za.zip");
            ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\7za.zip");
            ZipEntry SevenZip = zip["7za.exe"];
            SevenZip.Extract(LocalStorageFolder, ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateFFmpeg.Content = "Downloading FFmpeg...";
            }));
            ClientFFmpeg.DownloadFile("http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.7z", LocalStorageFolder + "\\ffmpeg.7z");
            Process SevenZipExtract = new Process();
            SevenZipExtract.StartInfo.CreateNoWindow = true;
            SevenZipExtract.StartInfo.UseShellExecute = false;
            SevenZipExtract.StartInfo.FileName = LocalStorageFolder + "\\7za.exe";
            SevenZipExtract.StartInfo.Arguments = "x -o\"" + ffmpegfolderPath + "\" \"" + LocalStorageFolder + "ffmpeg.7z\"";
            SevenZipExtract.Start();
            SevenZipExtract.WaitForExit();
            File.Delete(LocalStorageFolder + "\\7za.exe");

            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateFFmpeg.Content = "Moving files...";
            }));
            Process UpdateFFmpegProcess = new Process();
            UpdateFFmpegProcess.StartInfo.CreateNoWindow = true;
            UpdateFFmpegProcess.StartInfo.UseShellExecute = false;
            File.WriteAllText(LocalStorageFolder + "\\UpdateFFmpeg.ps1", "Move-Item \"" + ffmpegfolderPath + "\\ffmpeg-*\\*\" \"" + ffmpegfolderPath + "\"; Remove-Item -Force -Confirm:$false -Recurse \"" + ffmpegfolderPath + "\\ffmpeg-*\"");
            UpdateFFmpegProcess.StartInfo.FileName = "powershell.exe";
            UpdateFFmpegProcess.StartInfo.Arguments = " -exec bypass -File \"" + LocalStorageFolder + "\\UpdateFFmpeg.ps1\"";
            UpdateFFmpegProcess.Start();
            UpdateFFmpegProcess.WaitForExit();

            try { File.Delete(LocalStorageFolder + "UpdateFFmpeg.ps1"); }
            catch { MessageBox.Show("Unable to delete: " + LocalStorageFolder + "\\UpdateFFmpeg.ps1"); }

            try { File.Delete(LocalStorageFolder + "\\7za.zip"); }
            catch { MessageBox.Show("Unable to delete: " + LocalStorageFolder + "\\7za.zip"); }

            try { File.Delete(LocalStorageFolder + "\\ffmpeg.7z"); }
            catch { MessageBox.Show("Unable to delete: " + LocalStorageFolder + "\\ffmpeg.7z"); }


            File.WriteAllText(ffmpegfolderPath + "\\bin\\version.txt", LatestFFmpegVersion);
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateFFmpeg.Content = "Getting current version...";
            }));
            ffmpegGetCurrentVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateFFmpeg.Content = "Update finished";
            }));
            ffmpegUpdating = 0;
        }
        private void RTMPDumpInstallLastestVersion_Process()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                DownloadRTMPDump.Content = "Downloading RTMPdump..";
                DownloadRTMPDump.IsEnabled = false;
            }));
            ClientRTMPDump.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", LocalStorageFolder + "\\rtmpdump.zip");
            ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\rtmpdump.zip");
            ZipEntry rtmpdumpZip = zip["rtmpdump.exe"];
            rtmpdumpZip.Extract(LocalStorageFolder);
            zip.Dispose();
            File.Delete(LocalStorageFolder + "\\rtmpdump.zip");
            youtubedlGetCurrentVersion_Process();
            this.Dispatcher.Invoke((Action)(() =>
            {
                DownloadRTMPDump.Content = "Update finished";
            }));
            rtmpdumpUpdating = 0;
        }
        private void RTMPDumpCompareVersion_Process()
        {
                if (!File.Exists(RTMPDumpPath))
                {
                this.Dispatcher.Invoke((Action)(() => {
                    DownloadRTMPDump.Content = "Download RTMPDump";
                    DownloadRTMPDump.IsEnabled = true;
                }));
            }
        }
        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (youtubedlUpdating == 0  && ripmeUpdating == 0 && ffmpegUpdating == 0 && rtmpdumpUpdating == 0)
            {
                checkforUpdates = 1;
                this.Dispatcher.Invoke((Action)(() =>
                {
                CheckForUpdates.IsEnabled = false;
                }));
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
            checkforUpdates = 0;
        }
    }
}