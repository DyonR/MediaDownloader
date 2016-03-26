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

        BackgroundWorker youtubedlCurrentWorker;
        BackgroundWorker ffmpegCurrentWorker;
        BackgroundWorker ripmeCurrentWorker;
        BackgroundWorker RTMPDumpWorker;
        BackgroundWorker youtubedlUpdateWorker;
        BackgroundWorker ffmpegUpdateWorker;
        BackgroundWorker ripmeUpdateWorker;
        BackgroundWorker ffmpegDownloaderWorker;

        //I don't feel like placing comments in this file yet.
        //Will do it someday later.
        //I also need to clean up this section, probably.

        /*
                            processWorker.DoWork += (obj, ea) => ripMe_Process();
                processWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ripMe_Process_Complete);
                processWorker.RunWorkerAsync(); */

        WebClient WebClient = new WebClient();
        public string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + (@"\Media Downloads");
        public string LocalStorageFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\");
        public string YouTubeDLPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\youtube-dl.exe");
        public string RipMePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ripme.jar");
        public string ffmpegfolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ffmpeg");
        public string RTMPDumpPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\rtmpdump.exe");
        public string LatestFFmpegVersion;
        public string CurrentFFmpegVersion;



        private void update_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= update_Loaded;

            youtubedlCurrentWorker = new BackgroundWorker();
            youtubedlCurrentWorker.WorkerReportsProgress = true;
            youtubedlCurrentWorker.DoWork += (obj, ea) => youtubedlCurrentWorker_Process();
            youtubedlCurrentWorker.RunWorkerAsync();

            ffmpegCurrentWorker = new BackgroundWorker();
            ffmpegCurrentWorker.WorkerReportsProgress = true;
            ffmpegCurrentWorker.DoWork += (obj, ea) => ffmpegCurrentWorker_Process();
            ffmpegCurrentWorker.RunWorkerAsync();

            ripmeCurrentWorker = new BackgroundWorker();
            ripmeCurrentWorker.WorkerReportsProgress = true;
            ripmeCurrentWorker.DoWork += (obj, ea) => ripmeCurrentWorker_Process();
            ripmeCurrentWorker.RunWorkerAsync();

            RTMPDumpWorker = new BackgroundWorker();
            RTMPDumpWorker.WorkerReportsProgress = true;
            RTMPDumpWorker.DoWork += (obj, ea) => RTMPDumpWorker_Process();
            RTMPDumpWorker.RunWorkerAsync();
        }

        private void UpdateYouTubeDL_Click(object sender, RoutedEventArgs e)
        { 
            WebClient.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", YouTubeDLPath);
            Directory.SetCurrentDirectory(LocalStorageFolder);
            Process youtubedl = new Process();
            youtubedl.StartInfo.CreateNoWindow = true;
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.StartInfo.RedirectStandardError = true;
            youtubedl.StartInfo.FileName = YouTubeDLPath;
            youtubedl.StartInfo.Arguments = " --version";
            youtubedl.Start();
            string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
            CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
            UpdateYouTubeDL.IsEnabled = false;
            YouTubeDLVersionStatusText.Text = "Updated youtube-dl.exe";
            string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
            LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;

        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            WebClient.DownloadFile("http://rarchives.com/ripme.jar", RipMePath);
            Directory.SetCurrentDirectory(LocalStorageFolder);
            Process ripme = new Process();
            ripme.StartInfo.CreateNoWindow = true;
            ripme.StartInfo.UseShellExecute = false;
            ripme.StartInfo.RedirectStandardOutput = true;
            ripme.StartInfo.RedirectStandardError = true;
            ripme.StartInfo.FileName = "java";
            ripme.StartInfo.Arguments = " -jar \"" + RipMePath + "\" --help";
            ripme.Start();
            File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
            string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
            File.Delete("currentripme.txt");
            char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
            CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
            CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
            UpdateRipMe.IsEnabled = false;
            RipMeVersionStatusText.Text = "Updated RipMe";
            WebClient.DownloadFile("http://www.rarchives.com/ripme.json", LocalStorageFolder + "\\latestripme.txt");
            string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
            File.Delete("latestripme.txt");
            char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
            LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
            LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
        }

        private void UpdateFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            /*
            UpdateFFmpeg.IsEnabled = false;
            UpdateFFmpeg.Content = "Updating...";
            */
            ffmpegDownloaderWorker = new BackgroundWorker();
            ffmpegDownloaderWorker.DoWork += (obj, ea) => ffmpegDownloaderWorker_Process();
            ffmpegDownloaderWorker.RunWorkerAsync();
        }

          public void ffmpegDownloaderWorker_Process() {
            this.Dispatcher.Invoke((System.Action)(() => {
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
                WebClient.DownloadFile("http://www.7-zip.org/a/7za920.zip", LocalStorageFolder + "\\7za.zip");
                ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\7za.zip");
                ZipEntry SevenZip = zip["7za.exe"];
                SevenZip.Extract(LocalStorageFolder);
                zip.Dispose();
                WebClient.DownloadFile("http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.7z", LocalStorageFolder + "\\ffmpeg.7z");
                Process SevenZipExtract = new Process();
                SevenZipExtract.StartInfo.CreateNoWindow = true;
                SevenZipExtract.StartInfo.UseShellExecute = false;
                SevenZipExtract.StartInfo.FileName = LocalStorageFolder + "\\7za.exe";
                SevenZipExtract.StartInfo.Arguments = "x -o\"" + ffmpegfolderPath + "\" \"" + LocalStorageFolder +  "ffmpeg.7z\"";
                SevenZipExtract.Start();
                SevenZipExtract.WaitForExit();
                File.Delete(LocalStorageFolder + "\\7za.exe");

                Process UpdateFFmpegProcess = new Process();
                UpdateFFmpegProcess.StartInfo.CreateNoWindow = true;
                UpdateFFmpegProcess.StartInfo.UseShellExecute = false;
                File.WriteAllText(LocalStorageFolder + "\\UpdateFFmpeg.ps1","Move-Item \"" + ffmpegfolderPath + "\\ffmpeg-*\\*\" \"" + ffmpegfolderPath +"\"; Remove-Item -Force -Confirm:$false -Recurse \"" + ffmpegfolderPath + "\\ffmpeg-*\"");
                UpdateFFmpegProcess.StartInfo.FileName = "powershell.exe";
                UpdateFFmpegProcess.StartInfo.Arguments = " -exec bypass -File \"" + LocalStorageFolder + "\\UpdateFFmpeg.ps1\"";
                UpdateFFmpegProcess.Start();
                UpdateFFmpegProcess.WaitForExit();

                Process GetFFmpeg = new Process();
                GetFFmpeg.StartInfo.CreateNoWindow = true;
                GetFFmpeg.StartInfo.UseShellExecute = false;
                File.WriteAllText(LocalStorageFolder + "\\GetFFmpeg.ps1",
                 @"$DownloadsLocation = $env:LOCALAPPDATA + ""\Media Downloader""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
                GetFFmpeg.StartInfo.FileName = "powershell.exe";
                GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + LocalStorageFolder + "\\GetFFmpeg.ps1\"";
                GetFFmpeg.Start();
                GetFFmpeg.WaitForExit();
                string LatestFFmpegVersion = File.ReadLines(LocalStorageFolder + "LastFFmpeg.txt").First();
                LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
                File.WriteAllText(ffmpegfolderPath + "\\bin\\version.txt", LatestFFmpegVersion);
                string CurrentFFmpegVersion = File.ReadLines(ffmpegfolderPath + "\\bin\\version.txt").First();
                CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                UpdateFFmpeg.IsEnabled = false;
                FFmpegVersionStatusText.Text = "Updated FFmpeg";
                File.Delete(LocalStorageFolder + "\\GetFFmpeg.ps1");
                File.Delete(LocalStorageFolder + "\\UpdateFFmpeg.ps1");
                File.Delete(LocalStorageFolder + "\\LastFFmpeg.txt");
                File.Delete(LocalStorageFolder + "\\LatestFFmpeg.txt");
                File.Delete(LocalStorageFolder + "\\7za.zip");
                File.Delete(LocalStorageFolder + "\\ffmpeg.7z");
                UpdateFFmpeg.Content = "Update finished.";
            }));

        }
 

        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(RTMPDumpPath);
            WebClient.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", LocalStorageFolder + "\\rtmpdump.zip");
            ZipFile zip = ZipFile.Read(LocalStorageFolder + "\\rtmpdump.zip");
            ZipEntry rtmpdumpZip = zip["rtmpdump.exe"];
            rtmpdumpZip.Extract(LocalStorageFolder);
            zip.Dispose();
            File.Delete(LocalStorageFolder + "\\rtmpdump.zip");
            DownloadRTMPDump.IsEnabled = false;
        }
        
        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            youtubedlUpdateWorker = new BackgroundWorker();
            youtubedlUpdateWorker.WorkerReportsProgress = true;
            youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlUpdateWorker_Process();
            youtubedlUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(youtubedlUpdateWorker_Process_Complete);
            youtubedlUpdateWorker.RunWorkerAsync();
        }
        public void youtubedlCurrentWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
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
                string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
            }
                else {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + "youtube-dl.exe not found.";
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
                    UpdateYouTubeDL.IsEnabled = true;
                }
            }));
        }
        public void ffmpegCurrentWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
                if (File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
                {
                    string CurrentFFmpegVersion = File.ReadLines(ffmpegfolderPath + "\\bin\\version.txt").First();
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                }
                else {

                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + "ffmpeg not found.";
                    FFmpegVersionStatusText.Text = "ffmpeg can not be found, please click the button below.";
                    UpdateFFmpeg.IsEnabled = true;
                }
            }));
        }
        public void ripmeCurrentWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
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
                string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
                File.Delete("currentripme.txt");
                char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
                CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
                CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
            }
                else
                {
                    CurrentRipMeVersionText.Text = "Current RipMe version: " + "ripme.jar not found";
                    RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
                    UpdateRipMe.IsEnabled = true;
                }
            }));
        }
        public void RTMPDumpWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
                if (!File.Exists(RTMPDumpPath))
                {
                   DownloadRTMPDump.IsEnabled = true;
                }
            }));
        }
        public void youtubedlUpdateWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
                if (!File.Exists(YouTubeDLPath))
            {
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + "youtube-dl.exe not found";

                string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
                UpdateYouTubeDL.IsEnabled = true;
                YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
            }
            else {

                Directory.SetCurrentDirectory(LocalStorageFolder);

                //Here I get the current version of youtube-dl.exe, to get the version number, we have to run youtube-dl.exe --version
                Process youtubedl = new Process();
                youtubedl.StartInfo.CreateNoWindow = true;
                youtubedl.StartInfo.UseShellExecute = false;
                youtubedl.StartInfo.RedirectStandardOutput = true;
                youtubedl.StartInfo.RedirectStandardError = true;
                youtubedl.StartInfo.FileName = YouTubeDLPath;
                youtubedl.StartInfo.Arguments = " --version";
                youtubedl.Start();
                string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;

                //We also need to get the latest version, so we can compare the latest version with the current version
                string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;

                int YouTubeDLUptodate = CurrentYouTubeDLVersion.CompareTo(LatestYoutubeDLVersion);

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
            }));
        }
        public void ffmpegUpdateWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
                if (!File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
                {
                CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + "FFmpeg not found";

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
                string LatestFFmpegVersion = File.ReadAllText(@"LastFFmpeg.txt");

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
                LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;

                UpdateFFmpeg.IsEnabled = true;
                FFmpegVersionStatusText.Text = "FFmpeg can not be found, please click the button below.";
            }
            else {
                if (File.Exists(ffmpegfolderPath + "\\bin\\version.txt"))
                {
                    string CurrentFFmpegVersion = File.ReadLines(ffmpegfolderPath + "\\bin\\version.txt").First();
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
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
                    string LatestFFmpegVersion = File.ReadLines(@"LastFFmpeg.txt").First();

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
                    LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;

                    int FFmpegUptodate = CurrentFFmpegVersion.CompareTo(LatestFFmpegVersion);
                    //Higher version = 1
                    //Outdated = -1 
                    //Same version = 0
                    if (FFmpegUptodate == -1)
                    {
                        FFmpegVersionStatusText.Text = "Your FFmpeg is out of date, please click the button below to update.";
                        UpdateFFmpeg.IsEnabled = true;
                    }
                    else
                    {
                        FFmpegVersionStatusText.Text = "FFmpeg is up to date!";

                    }
                }
                else
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
                    string LatestFFmpegVersion = File.ReadAllText(@"LastFFmpeg.txt");

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
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + "Error getting current version.";
                    LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
                    FFmpegVersionStatusText.Text = "Error getting version, please update.";
                    UpdateFFmpeg.IsEnabled = true;
                }

            }
            }));
        }
        public void ripmeUpdateWorker_Process()
        {
            this.Dispatcher.Invoke((System.Action)(() => {
                if (!File.Exists(RipMePath))
            {

                CurrentRipMeVersionText.Text = "Current RipMe version: " + "ripme.jar not found";
                WebClient.DownloadFile("http://www.rarchives.com/ripme.json", LocalStorageFolder + "\\latestripme.txt");
                string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
                File.Delete("latestripme.txt");
                char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
                LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
                LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
                UpdateRipMe.IsEnabled = true;
                RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
            }
            else {
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
                string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
                File.Delete("currentripme.txt");
                char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
                CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
                CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;

                //Getting the latest RipMe Version
                WebClient.DownloadFile("http://www.rarchives.com/ripme.json", LocalStorageFolder + "\\latestripme.txt");
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
            }));
        }
        private void youtubedlUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ripmeUpdateWorker = new BackgroundWorker();
            ripmeUpdateWorker.WorkerReportsProgress = true;
            ripmeUpdateWorker.DoWork += (obj, ea) => ripmeUpdateWorker_Process();
            ripmeUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ripmeUpdateWorker_Process_Complete);
            ripmeUpdateWorker.RunWorkerAsync();
        }
        private void ripmeUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ffmpegUpdateWorker = new BackgroundWorker();
            ffmpegUpdateWorker.WorkerReportsProgress = true;
            ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegUpdateWorker_Process();
            ffmpegUpdateWorker.RunWorkerAsync();
        }
    }
}